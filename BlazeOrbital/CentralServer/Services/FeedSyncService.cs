using System.Globalization;
using BlazeOrbital.CentralServer.Data;
using CsvHelper;

namespace BlazeOrbital.Data.Services;

public class FeedSyncService: BackgroundService
{
    public readonly IServiceScopeFactory ScopeFactory;
   // private const string FeedUrl = "https://transport.productsup.io/9749eccfe150b21a58b0/channel/378317/pdsfeed.csv";
   private const string FeedUrl = "https://transport.productsup.io/9749eccfe150b21a58b0/channel/377786/pdsfeed.csv";


    public FeedSyncService(IServiceScopeFactory scopeFactory)
    {
        ScopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await ReadRemoteFeedAsync(db, stoppingToken);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private Task ReadRemoteFeedAsync(ApplicationDbContext db, CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            try
            {
                int index = 0;
                Uri uri = new Uri(FeedUrl);

                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
                var webResponse = request.GetResponse();
                if ((webResponse.ContentLength > 0))
                {
                    StreamReader strReader = new StreamReader(webResponse.GetResponseStream());
                    CsvReader csv = new CsvReader(strReader, CultureInfo.InvariantCulture);
                    //create datatable with column names
                    string? line;
                    while (csv.Read())
                    {
                        if (index > 0) // header we don't want
                        {
                            var title = csv.GetField<string>(0);
                            var id = csv.GetField<string>(1);
                            var image = csv.GetField<string>(2);
                            var brand = csv.GetField<string>(3);
                            var targetUrl = csv.GetField<string>(4);
                            var thumbNail = csv.GetField<string>(5);
                            var category = csv.GetField<string>(6);
                            var product = csv.GetField<string>(7);
                            var salePercentage = csv.GetField<string>(8);
                            var price = csv.GetField<string>(9);
                            var salePrice = csv.GetField<string>(10);
                            var shop = csv.GetField<string>(11);
                            var category2 = csv.GetField<string>(12);


                            if (index % 1000 == 0)
                            {
                                Console.WriteLine($"Seeded item {index}...");
                            }
                        }

                        index++;
                    }

                    strReader.Close();
                }
            }
            catch (System.Net.WebException ex)
            {

            }
        }, stoppingToken);
    }

    private void ReadAndUpdate(ApplicationDbContext db, string line)
    {
        
    }
}