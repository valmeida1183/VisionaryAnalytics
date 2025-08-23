using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Storage;
public interface IVideoStorageService
{
    Task CreateVideoFileAsync(IFormFile file, 
                              Guid fileId, 
                              string fileName, 
                              CancellationToken cancellationToken);
    string GetVideoFilePath(Guid fileId, string fileName);
    string GetVideoFolderPath(Guid fileId);
}
