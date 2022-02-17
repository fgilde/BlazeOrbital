using System.Data;
using System.Data.Common;
using System.Reflection;

namespace QuickGrid.EF;

public class Bulk<T>
{
    private readonly DbConnection _connection;
    private readonly string _tableName;
    private readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();
    private IDictionary<Type, DbType> _typeMap;
    public Bulk(DbConnection connection, string tableName = nameof(T))
    {
        _connection = connection;
        _tableName = tableName;
        SetTypeMap();
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

    public Bulk<T> SetTypeMap(IDictionary<Type, DbType>? typeMap = null)
    {
        _typeMap = typeMap ?? new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset
        };
        return this;
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
                    {
                        param.Value = property.GetValue(item);
                        param.DbType = GeDbType(param.Value?.GetType());
                    }

                }
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        return this;
    }

    public virtual DbType GeDbType(Type? type)
    {
        return type != null && _typeMap?.ContainsKey(type) == true ? _typeMap[type] : default;
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
