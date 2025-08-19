namespace Domain.Abstractions.Repositories;
public interface IVideoProcessResultRepository
{
    Task<IEnumerable<VideoProcessResult>> GetAllAsync();
    Task<VideoProcessResult?> GetByIdAsync(Guid id);

    Task AddAsync(VideoProcessResult videoProcessResult);
    void Update(VideoProcessResult videoProcessResult);
    void Delete(VideoProcessResult videoProcessResult);
}
