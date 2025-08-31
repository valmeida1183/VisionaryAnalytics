using Domain.Entities;

namespace Application.Abstractions.Repositories;
public interface IVideoProcessRepository
{
    Task<IEnumerable<VideoProcess>> GetAllAsync(CancellationToken cancellationToken);
    Task<VideoProcess?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(VideoProcess videoProcess);
    void Update(VideoProcess videoProcess);
    void Delete(VideoProcess videoProcess);
}
