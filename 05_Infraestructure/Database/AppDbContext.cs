using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Database;
public sealed class AppDbContext : DbContext
{
    public DbSet<VideoProcessResult> VideoProcessResults { get; set; }
    public DbSet<VideoQRCode> VideoQRCodes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
