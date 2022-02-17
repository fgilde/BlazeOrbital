using Cherry.CentralServer.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

// Dont change this namespace it will be partial on client generator
namespace Cherry.Data;

//[Authorize]
public class CherryDataService : CherryData.CherryDataBase
{
    private readonly ApplicationDbContext db;

    public CherryDataService(ApplicationDbContext db)
    {
        this.db = db;
    }

    public override Task<DashboardReply> GetDashboardData(DashboardRequest request, ServerCallContext context)
    {
        return Task.FromResult(new DashboardReply
        {
            ProjectsBookedValue = 38_000_000,
            NextDeliveryDueInMs = (long)TimeSpan.FromHours(53).TotalMilliseconds,
            StaffOnSite = 441,
            FactoryUptimeMs = (long)TimeSpan.FromDays(152).TotalMilliseconds,
            ServicingTasksDue = 7,
            MachinesStopped = 3,
        });
    }

    public override async Task<ProductsReply> GetProducts(ProductsRequest request, ServerCallContext context)
    {
        var modifiedParts = db.Products
            .OrderBy(p => p.DateUpdated)
            .Where(p => p.DateUpdated > request.ModifiedSince);
         //
        var reply = new ProductsReply();
        reply.ModifiedCount = await modifiedParts.CountAsync();
        reply.Products.AddRange(await modifiedParts.Take(request.MaxCount).ToListAsync());
        return reply;
    }
}
