using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Runtime.InteropServices;
using QuickGrid.EF;

namespace QuickGrid.EF;

// This service synchronizes the Sqlite DB with both the backend server and the browser's IndexedDb storage
public abstract class NewDataSynchronizer<TDbContext, TEntity> 
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly Task firstTimeSetupTask;
    private readonly IDbContextFactory<TDbContext> dbContextFactory;

    public bool IsSynchronizing { get; private set; }
    public int SyncCompleted { get; private set; }
    public int SyncTotal { get; private set; }
    public event Action? OnUpdate;
    public event Action<Exception>? OnError;

    protected NewDataSynchronizer(IJSRuntime js, IDbContextFactory<TDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
        firstTimeSetupTask = FirstTimeSetupAsync(js);
    }
    
    public async Task<TDbContext> GetPreparedDbContextAsync()
    {
        await firstTimeSetupTask;
        return await dbContextFactory.CreateDbContextAsync();
    }

    public void SynchronizeInBackground()
    {
        _ = EnsureSynchronizingAsync();
    }

    protected abstract Task<(IEnumerable<TEntity> Result, int Total)> GetEntitiesFromServerAsync(TEntity? lastSynced = null);

    private async Task FirstTimeSetupAsync(IJSRuntime js)
    {
        var module = await js.InvokeAsync<IJSObjectReference>("import", "./dbstorage.js");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
            await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", Settings.SqliteDbFilename);

        await using var db = await dbContextFactory.CreateDbContextAsync();
        // db.Database.EnsureDeleted();
        await db.Database.EnsureCreatedAsync();
    }

    private async Task EnsureSynchronizingAsync()
    {
        // Don't run multiple syncs in parallel. This simple logic is adequate because of single-threadedness.
        if (IsSynchronizing)
            return;
        
        try
        {
            IsSynchronizing = true;
            SyncCompleted = 0;
            SyncTotal = 0;

            // Get a DB context
            using var db = await GetPreparedDbContextAsync();
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            // Begin fetching any updates to the dataset from the backend server
            TEntity? last = null;
            var connection = db.Database.GetDbConnection();
            await connection.OpenAsync();
            var table = db.GetTableName<TEntity>();
            var bulk = new Bulk<TEntity>(connection, table);

            while (true)
            {
                var items = await GetEntitiesFromServerAsync(last);
                var resultCount = items.Result.TryGetNonEnumeratedCount(out var count) ? count : items.Result.Count();
                var syncRemaining = items.Total - resultCount;

                SyncCompleted += resultCount;
                SyncTotal = SyncCompleted + syncRemaining;

                if (resultCount <= 0)
                {
                    break;
                }

                last = items.Result.LastOrDefault();
                bulk.InsertOrUpdate(items.Result);
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
            IsSynchronizing = false;
        }
    }

}
