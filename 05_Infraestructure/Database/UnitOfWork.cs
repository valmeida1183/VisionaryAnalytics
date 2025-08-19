using Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions;

namespace Infraestructure.Database;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        return _context.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e.Entity is Entity && 
                        (e.State == EntityState.Added));
        foreach (var entry in entries)
        {
            var auditableEntity = (Entity)entry.Entity;
            auditableEntity.CreatedOn = DateTime.UtcNow;
        }
    }
}
