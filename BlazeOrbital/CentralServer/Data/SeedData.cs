using BlazeOrbital.Data;
using CsvHelper;
using System.Globalization;

namespace BlazeOrbital.CentralServer.Data;

public static class SeedData
{
    private const string FeedUrl = "https://transport.productsup.io/9749eccfe150b21a58b0/channel/378317/pdsfeed.csv";

    public static void EnsureSeeded(IServiceProvider services)
    {
        var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (db.Database.EnsureCreated())
        {
            ReadLocalFeedData(db);
            db.SaveChanges();
        }
    }


    private static void ReadLocalFeedData(ApplicationDbContext db)
    {
        var dir = Path.GetDirectoryName(typeof(SeedData).Assembly.Location)!;
        using var fileReader = (TextReader) File.OpenText(Path.Combine(dir, "Data", "parts.csv"));
        using var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
        var count = 0;
        while (csv.Read())
        {
            count++;

            db.Parts.Add(new Part
            {
                PartId = count,
                Category = csv.GetField<string>(0),
                Subcategory = csv.GetField<string>(1),
                Name = csv.GetField<string>(2),
                Location = csv.GetField<string>(3),
                PriceCents = (long) (csv.GetField<double>(4) * 100),
                ModifiedTicks = count,
                Stock = (int) Math.Round(50000 * Random.Shared.NextDouble() * Random.Shared.NextDouble() *
                                         Random.Shared.NextDouble()),
            });

            if (count % 1000 == 0)
            {
                Console.WriteLine($"Seeded item {count}...");
            }
        }
    }
}
