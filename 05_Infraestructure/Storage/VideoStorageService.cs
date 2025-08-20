using Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;

namespace Infraestructure.Storage;
public class VideoStorageService : IVideoStorageService
{
    private readonly string _storagePath;

    public VideoStorageService(string storagePath)
    {
        _storagePath = storagePath;

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task CreateVideoFileAsync(IFormFile file, string fileName, CancellationToken cancellationToken)
    {
       var filePath = Path.Combine(_storagePath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        
        await file.CopyToAsync(stream, cancellationToken);
    }
}
