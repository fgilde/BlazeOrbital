using BlazeOrbital.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace BlazeOrbital.ManufacturingHub.Data;

// This service synchronizes the Sqlite DB with both the backend server and the browser's IndexedDb storage
class DataSynchronizer
{
    public const string SqliteDbFilename = "app.db";
    private readonly Task firstTimeSetupTask;
    private readonly IDbContextFactory<ClientSideDbContext> dbContextFactory;
    private readonly ManufacturingData.ManufacturingDataClient manufacturingData;
    private bool isSynchronizing;

    public DataSynchronizer(IJSRuntime js, IDbContextFactory<ClientSideDbContext> dbContextFactory, ManufacturingData.ManufacturingDataClient manufacturingData)
    {
        this.dbContextFactory = dbContextFactory;
        this.manufacturingData = manufacturingData;
        firstTimeSetupTask = FirstTimeSetupAsync(js);
    }

    public int SyncCompleted { get; private set; }
    public int SyncTotal { get; private set; }

    public async Task<ClientSideDbContext> GetPreparedDbContextAsync()
    {
        await firstTimeSetupTask;
        return await dbContextFactory.CreateDbContextAsync();
    }

    public void SynchronizeInBackground()
    {
        _ = EnsureSynchronizingAsync();
    }

    public event Action? OnUpdate;
    public event Action<Exception>? OnError;

    private async Task FirstTimeSetupAsync(IJSRuntime js)
    {
        var module = await js.InvokeAsync<IJSObjectReference>("import", "./dbstorage.js");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
        {
            await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", SqliteDbFilename);
        }

        using var db = await dbContextFactory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
    }

    private async Task EnsureSynchronizingAsync()
    {
        // Don't run multiple syncs in parallel. This simple logic is adequate because of single-threadedness.
        if (isSynchronizing)
        {
            return;
        }

        try
        {
            isSynchronizing = true;
            SyncCompleted = 0;
            SyncTotal = 0;

            // Get a DB context
            using var db = await GetPreparedDbContextAsync();
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var l = db.Products.FirstOrDefault();

            // Begin fetching any updates to the dataset from the backend server
            var mostRecentUpdate = db.Products.OrderByDescending(p => p.ModifiedTicks).FirstOrDefault()?.ModifiedTicks;

            var connection = db.Database.GetDbConnection();
            connection.Open();

            while (true)
            {
                var request = new ProductsRequest { MaxCount = 5000, ModifiedSinceTicks = mostRecentUpdate ?? -1 };
                ProductsReply? response = null;

                try
                {
                    response = await manufacturingData.GetProductsAsync(request);
                }
                catch (Exception e)
                {
                    
                }

                var syncRemaining = response.ModifiedCount - response.Products.Count;
                SyncCompleted += response.Products.Count;
                SyncTotal = SyncCompleted + syncRemaining;

                if (response.Products.Count == 0)
                {
                    break;
                }
                else
                {
                    mostRecentUpdate = response.Products.Last().ModifiedTicks;
                    BulkInsert(connection, response.Products);
                    OnUpdate?.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            // TODO: use logger also
            OnError?.Invoke(ex);
        }
        finally
        {
            isSynchronizing = false;
        }
    }

    private void BulkInsert(DbConnection connection, IEnumerable<Product> parts)
    {
        // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
        // using the fastest bulk insertion technique for Sqlite.
        using (var transaction = connection.BeginTransaction())
        {
            var command = connection.CreateCommand();
            var id = AddNamedParameter(command, "$Id");
            var category = AddNamedParameter(command, "$Category");
            var subcategory = AddNamedParameter(command, "$Subcategory");
            var name = AddNamedParameter(command, "$Name");
            var location = AddNamedParameter(command, "$Location");
            var image = AddNamedParameter(command, "$Image");
            var brand = AddNamedParameter(command, "$Brand");
            var targetUrl = AddNamedParameter(command, "$TargetUrl");
            var thumbnail = AddNamedParameter(command, "$Thumbnail");
            var product = AddNamedParameter(command, "$Product");
            var salePercentage = AddNamedParameter(command, "$SalePercentage");
            var price = AddNamedParameter(command, "$Price");
            var salePrice = AddNamedParameter(command, "$SalePrice");
            var shop = AddNamedParameter(command, "$Shop");
            var created = AddNamedParameter(command, "$DateCreated");
            var updated = AddNamedParameter(command, "$DateUpdated");
            var modifiedTicks = AddNamedParameter(command, "$ModifiedTicks");

            command.CommandText =
                $"INSERT OR REPLACE INTO Products ({nameof(Product.Id)}, {nameof(Product.Category)}, {nameof(Product.Subcategory)}, {nameof(Product.Name)}, {nameof(Product.Location)}, {nameof(Product.Image)}, {nameof(Product.Brand)}, {nameof(Product.TargetUrl)}, {nameof(Product.Thumbnail)}, {nameof(Product.Product_)}, {nameof(Product.SalePercentage)}, {nameof(Product.Price)}, {nameof(Product.SalePrice)}, {nameof(Product.Shop)}, {nameof(Product.DateCreated)}, {nameof(Product.DateUpdated)}, {nameof(Product.ModifiedTicks)}) " +
                $"VALUES ({id.ParameterName}, {category.ParameterName}, {subcategory.ParameterName}, {name.ParameterName}, {location.ParameterName}, {image.ParameterName}, {brand.ParameterName}, {targetUrl.ParameterName}, {thumbnail.ParameterName}, {product.ParameterName}, {salePercentage.ParameterName}, {price.ParameterName}, {salePrice.ParameterName}, {shop.ParameterName}, {created.ParameterName}, {updated.ParameterName}, {modifiedTicks.ParameterName})";

            foreach (var part in parts)
            {
                id.Value = part.Id;
                category.Value = part.Category;
                subcategory.Value = part.Subcategory;
                name.Value = part.Name;
                location.Value = part.Location;
                image.Value = part.Image;
                brand.Value = part.Brand;
                targetUrl.Value = part.TargetUrl;
                shop.Value = part.Shop;
                price.Value = part.Price;
                salePercentage.Value = part.SalePercentage;
                thumbnail.Value = part.Thumbnail;
                product.Value = part.Product_;
                salePrice.Value = part.SalePrice;
                created.Value = part.DateCreated;
                updated.Value = part.DateUpdated;
                modifiedTicks.Value = part.ModifiedTicks;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        static DbParameter AddNamedParameter(DbCommand command, string name)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
