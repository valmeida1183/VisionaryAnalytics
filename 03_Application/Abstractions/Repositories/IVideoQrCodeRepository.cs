using Domain.Entities;

namespace Application.Abstractions.Repositories;
public interface IVideoQrCodeRepository
{
    Task<IEnumerable<VideoQRCode>> GetByVideoProcessIdAsync(Guid videoProcessId, CancellationToken cancellationToken);
    Task<VideoQRCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(VideoQRCode videoQRCode);
    Task AddRangeAsync(IEnumerable<VideoQRCode> videoQRCodes);
    void Update(VideoQRCode videoQRCode);
    void Delete(VideoQRCode videoQRCode);
}
