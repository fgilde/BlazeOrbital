using Cherry.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Runtime.InteropServices;

namespace Cherry.Client.Data;

// This service synchronizes the Sqlite DB with both the backend server and the browser's IndexedDb storage
class DataSynchronizer
{
    public const string SqliteDbFilename = "app.db";
    private readonly Task firstTimeSetupTask;
    private readonly IDbContextFactory<ClientSideDbContext> dbContextFactory;
    private readonly CherryData.CherryDataClient cherryDataClient;
    private bool isSynchronizing;

    public DataSynchronizer(IJSRuntime js, IDbContextFactory<ClientSideDbContext> dbContextFactory, CherryData.CherryDataClient cherryDataClient)
    {
        this.dbContextFactory = dbContextFactory;
        this.cherryDataClient = cherryDataClient;
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
            await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", SqliteDbFilename);


        await using var db = await dbContextFactory.CreateDbContextAsync();
       // db.Database.EnsureDeleted();
        await db.Database.EnsureCreatedAsync();
    }

    private async Task EnsureSynchronizingAsync()
    {
        // Don't run multiple syncs in parallel. This simple logic is adequate because of single-threadedness.
        if (isSynchronizing)
            return;
        
        try
        {
            isSynchronizing = true;
            SyncCompleted = 0;
            SyncTotal = 0;

            // Get a DB context
            using var db = await GetPreparedDbContextAsync();
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            // Begin fetching any updates to the dataset from the backend server
            var mostRecentUpdate = db.Products.OrderByDescending(p => p.DateUpdated).FirstOrDefault()?.DateUpdated;

            var connection = db.Database.GetDbConnection();
            await connection.OpenAsync();
            var bulk = new Bulk<Product>(connection, nameof(db.Products));

            while (true)
            {
                var request = new ProductsRequest { MaxCount = 5000, ModifiedSince = mostRecentUpdate ?? -1 };
                var response = await cherryDataClient.GetProductsAsync(request);
                var syncRemaining = response.ModifiedCount - response.Products.Count;

                SyncCompleted += response.Products.Count;
                SyncTotal = SyncCompleted + syncRemaining;

                if (response.Products.Count == 0)
                {
                    break;
                }

                mostRecentUpdate = response.Products.Last().DateUpdated;
                bulk.InsertOrUpdate(response.Products);
                OnUpdate?.Invoke();
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

}
