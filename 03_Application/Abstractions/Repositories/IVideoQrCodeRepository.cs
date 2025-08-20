using Domain.Entities;

namespace Application.Abstractions.Repositories;
public interface IVideoQrCodeRepository
{
    Task<IEnumerable<VideoQRCode>> GetAllAsync();
    Task<VideoQRCode?> GetByIdAsync(Guid id);
    Task AddAsync(VideoQRCode videoQRCode);
    void Update(VideoQRCode videoQRCode);
    void Delete(VideoQRCode videoQRCode);
}
