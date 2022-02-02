using System.Globalization;
using BlazeOrbital.CentralServer.Data;
using CsvHelper;

namespace BlazeOrbital.Data.Services;

public class FeedSyncService : BackgroundService
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
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
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
                    while (csv.Read())
                    {
                        if (index > 0) // header we don't want
                            AddOrUpdateProduct(CreateProduct(csv), db);
                        index++;
                    }

                    strReader.Close();
                    db.SaveChanges();
                }
            }
            catch (System.Net.WebException ex)
            {

            }
        }, stoppingToken);
    }

    private void AddOrUpdateProduct(Product? product, ApplicationDbContext db)
    {
        if (product == null)
            return;
        var productInDb = db.Products.FirstOrDefault(x => x.Id == product.Id);
        if (productInDb != null)
        {
            // Update
            product.Updated = DateTime.UtcNow;
            product.Created = productInDb.Created;
            db.Entry(productInDb).CurrentValues.SetValues(product);
        }
        else
        {
            product.Updated = DateTime.UtcNow;
            product.Created = DateTime.UtcNow;
            db.Products.Add(product);
        }
    }


    private Product? CreateProduct(CsvReader csv)
    {
        return new Product()
        {
            Name = csv.GetField<string>(0),
            Id = csv.GetField<string>(1),
            Image = csv.GetField<string>(2),
            Brand = csv.GetField<string>(3),
            TargetUrl = csv.GetField<string>(4),
            Thumbnail = csv.GetField<string>(5),
            Category = csv.GetField<string>(6),
            Product_ = csv.GetField<string>(7),
            SalePercentage = csv.GetField<string>(8),
            Price = csv.GetField<string>(9),
            SalePrice = csv.GetField<string>(10),
            Shop = csv.GetField<string>(11),
            Subcategory = csv.GetField<string>(12)
        };
    }

}