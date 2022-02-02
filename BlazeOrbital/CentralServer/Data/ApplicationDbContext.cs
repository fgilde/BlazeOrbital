using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using BlazeOrbital.Data;

namespace BlazeOrbital.CentralServer.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<IdentityUser>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Product>().HasKey(nameof(Product.Id));
        builder.Entity<Product>().HasIndex(nameof(Product.ModifiedTicks), nameof(Product.Id));
        builder.Entity<Product>().Ignore(p => p.Created);
        builder.Entity<Product>().Ignore(p => p.Updated);
    }

    // Inventory
    public DbSet<Product> Products { get; set; } = default!;
}
