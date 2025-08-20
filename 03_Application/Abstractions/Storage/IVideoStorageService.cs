using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Storage;
public interface IVideoStorageService
{
    Task CreateVideoFileAsync(IFormFile file, string fileName, CancellationToken cancellationToken);
}
