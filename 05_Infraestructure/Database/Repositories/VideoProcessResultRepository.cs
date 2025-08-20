using Application.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Database.Repositories;
public class VideoProcessResultRepository : IVideoProcessResultRepository
{
    private readonly AppDbContext _appDbContext;

    public VideoProcessResultRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<VideoProcessResult>> GetAllAsync()
    {
        return await _appDbContext
            .VideoProcessResults
            .ToListAsync();
    }

    public async Task<VideoProcessResult?> GetByIdAsync(Guid id)
    {
        return await _appDbContext
            .VideoProcessResults
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(VideoProcessResult videoProcessResult)
    {
        await _appDbContext
            .VideoProcessResults
            .AddAsync(videoProcessResult);
    }

    public void Update(VideoProcessResult videoProcessResult)
    {
        _appDbContext
            .VideoProcessResults
            .Update(videoProcessResult);
    }

    public void Delete(VideoProcessResult videoProcessResult)
    {
        _appDbContext
            .VideoProcessResults
            .Remove(videoProcessResult);
    }
}
