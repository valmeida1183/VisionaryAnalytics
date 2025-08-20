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

    public async Task<IEnumerable<VideoQRCode>> GetAllAsync()
    {
        return await _appDbContext
            .VideoQRCodes
            .ToListAsync();
    }

    public async Task<VideoQRCode?> GetByIdAsync(Guid id)
    {
        return await _appDbContext
            .VideoQRCodes
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(VideoQRCode videoQRCode)
    {
        await _appDbContext
            .VideoQRCodes
            .AddAsync(videoQRCode);
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
