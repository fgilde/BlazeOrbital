using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace QuickGrid.EF;

public static class ServiceExtensions
{
    public static IServiceCollection AddDataSynchronizer<TDbContext>(this IServiceCollection serviceCollection) 
        where TDbContext: DbContext
    {
        serviceCollection.AddDbContextFactory<TDbContext>(
            options => options.UseSqlite($"Filename={Settings.SqliteDbFilename}"));
        throw new NotImplementedException();
        //serviceCollection.AddScoped<NewDataSynchronizer>();
        return serviceCollection;
    }

    public static IServiceCollection AddDataSynchronizer(this IServiceCollection serviceCollection, Action<ModelBuilder>? onModelCreating = null)
    {
        ClientSideDbContext.OnModelCreation = onModelCreating;
        return serviceCollection.AddDataSynchronizer<ClientSideDbContext>();
    }
}