using Microsoft.EntityFrameworkCore;

namespace QuickGrid.EF;

public class ClientSideDbContext : DbContext
{
    internal static Action<ModelBuilder>? OnModelCreation { get; set; } 

    public ClientSideDbContext(DbContextOptions<ClientSideDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        if(OnModelCreation != null)
            OnModelCreation(modelBuilder);
    }
}
