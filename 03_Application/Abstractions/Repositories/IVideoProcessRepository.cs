using Domain.Entities;

namespace Application.Abstractions.Repositories;
public interface IVideoProcessRepository
{
    Task<IEnumerable<VideoProcess>> GetAllAsync();
    Task<VideoProcess?> GetByIdAsync(Guid id);

    Task AddAsync(VideoProcess videoProcess);
    void Update(VideoProcess videoProcess);
    void Delete(VideoProcess videoProcess);
}
