using Application.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Database.Repositories;
public class VideoQrCodeRepository : IVideoQrCodeRepository
{
    private readonly AppDbContext _appDbContext;

    public VideoQrCodeRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<VideoQRCode>> GetByVideoProcessIdAsync(Guid videoProcessId, CancellationToken cancellationToken)
    {
        return await _appDbContext
            .VideoQRCodes
            .Where(x => x.VideoProcessId == videoProcessId)
            .ToListAsync(cancellationToken);
    }

    public async Task<VideoQRCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _appDbContext
            .VideoQRCodes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(VideoQRCode videoQRCode)
    {
        await _appDbContext
            .VideoQRCodes
            .AddAsync(videoQRCode);
    }

    public async Task AddRangeAsync(IEnumerable<VideoQRCode> videoQRCodes)
    {
        await _appDbContext
            .VideoQRCodes
            .AddRangeAsync(videoQRCodes);
    }

    public void Update(VideoQRCode videoQRCode)
    {
        _appDbContext
            .VideoQRCodes
            .Update(videoQRCode);
    }

    public void Delete(VideoQRCode videoQRCode)
    {
        _appDbContext
            .VideoQRCodes
            .Remove(videoQRCode);
    }
}
