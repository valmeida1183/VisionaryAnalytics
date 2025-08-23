using Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;

namespace Infraestructure.Storage;
public class VideoStorageService : IVideoStorageService
{
    private readonly string _storagePath;
    public string StoragePath => _storagePath;

    public VideoStorageService(string storagePath)
    {
        _storagePath = storagePath;

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task CreateVideoFileAsync(IFormFile file, Guid fileId, string fileName, CancellationToken cancellationToken)
    {
        var fileFolder = $"{_storagePath}/{fileId}";

        if (!Directory.Exists(fileFolder))
        {
            Directory.CreateDirectory(fileFolder);
        }

        var filePath = Path.Combine(fileFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream, cancellationToken);
    }
}
