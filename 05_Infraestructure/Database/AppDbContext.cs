using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Database;
public sealed class AppDbContext : DbContext
{
    public DbSet<VideoProcess> VideoProcess { get; set; }
    public DbSet<VideoQRCode> VideoQRCodes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
