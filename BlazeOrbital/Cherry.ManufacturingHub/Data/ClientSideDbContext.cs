using Cherry.Data;
using Microsoft.EntityFrameworkCore;

namespace Cherry.Client.Data;

internal class ClientSideDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = default!;

    public ClientSideDbContext(DbContextOptions<ClientSideDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<Product>().HasKey(nameof(Product.Id));
        modelBuilder.Entity<Product>().HasIndex(nameof(Product.ModifiedTicks), nameof(Product.Id));
        modelBuilder.Entity<Product>().HasIndex(nameof(Product.Category), nameof(Product.Subcategory));
        modelBuilder.Entity<Product>().HasIndex(x => x.Name);
        modelBuilder.Entity<Product>().Property(x => x.Name).UseCollation("nocase");
    }
}
