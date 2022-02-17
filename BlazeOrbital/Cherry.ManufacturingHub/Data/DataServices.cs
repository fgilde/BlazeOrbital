using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.EntityFrameworkCore;

namespace Cherry.Client.Data;

public static class DataServices
{
    public static void AddDataClient(this IServiceCollection serviceCollection, Action<IServiceProvider, CherryDataClientOptions> configure)
    {
        serviceCollection.AddScoped(services =>
        {
            var options = new CherryDataClientOptions();
            configure(services, options);
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, options.MessageHandler!));
            var channel = GrpcChannel.ForAddress(options.BaseUri!, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null });
            return new Cherry.Data.CherryData.CherryDataClient(channel);
        });
    }

    public static void AddDataDbContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContextFactory<ClientSideDbContext>(
            options => options.UseSqlite($"Filename={DataSynchronizer.SqliteDbFilename}"));
        serviceCollection.AddScoped<DataSynchronizer>();
    }
}

public class CherryDataClientOptions
{
    public string? BaseUri { get; set; }
    public HttpMessageHandler? MessageHandler { get; set; }
}
