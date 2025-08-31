using Application.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Database.Repositories;
public class VideoProcessRepository : IVideoProcessRepository
{
    private readonly AppDbContext _appDbContext;

    public VideoProcessRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<VideoProcess>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _appDbContext
            .VideoProcess
            .ToListAsync(cancellationToken);
    }

    public async Task<VideoProcess?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _appDbContext
            .VideoProcess
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(VideoProcess videoProcess)
    {
        await _appDbContext
            .VideoProcess
            .AddAsync(videoProcess);
    }

    public void Update(VideoProcess videoProcess)
    {
        _appDbContext
            .VideoProcess
            .Update(videoProcess);
    }

    public void Delete(VideoProcess videoProcess)
    {
        _appDbContext
            .VideoProcess
            .Remove(videoProcess);
    }
}
