using System.Data.Common;
using System.Reflection;
using Cherry.Data;

namespace Cherry.ManufacturingHub.Data;

public class Bulk<T>
{
    private readonly DbConnection _connection;
    private readonly string _tableName;
    private readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

    public Bulk(DbConnection connection, string tableName = nameof(T))
    {
        _connection = connection;
        _tableName = tableName;
    }


    private PropertyInfo[] PropertiesFor()
    {
        if(_propertyCache.ContainsKey(typeof(T)))
            return _propertyCache[typeof(T)];
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i => i.CanWrite);

        var result = properties.ToArray();
        _propertyCache.Add(typeof(T), result);
        return result;
    }

    public Bulk<T> InsertOrUpdate(IEnumerable<T> items)
    {
        // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
        // using the fastest bulk insertion technique for Sqlite.

        var properties = PropertiesFor();
        var propertyNames = properties.Select(p => p.Name).ToArray();
        
        using (var transaction = _connection.BeginTransaction())
        {
            var command = _connection.CreateCommand();
            var commandParams = AddNamedParameters(command, propertyNames.Select(n => "$" + n).ToArray()).ToArray();

            command.CommandText =
                $"INSERT OR REPLACE INTO {_tableName} ({string.Join(", ", propertyNames)}) " +
                $"VALUES ({string.Join(", ", commandParams.Select(p => p.ParameterName))})";

            foreach (var item in items)
            {
                foreach (var param in commandParams)
                {
                    var property = properties.FirstOrDefault(p => p.Name == param.ParameterName.Substring(1)); // Skip the $
                    if (property != null)
                        param.Value = property.GetValue(item);
                }
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        return this;
    }

    public static IEnumerable<DbParameter> AddNamedParameters(DbCommand command, params string[] names)
    {
        return names.Select(name => AddNamedParameter(command, name));
    }

    public static DbParameter AddNamedParameter(DbCommand command, string name)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        command.Parameters.Add(parameter);
        return parameter;
    }
}
