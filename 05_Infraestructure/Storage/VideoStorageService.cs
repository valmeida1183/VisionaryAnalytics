using Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using SharedKernel.Primitives;

namespace Infraestructure.Storage;
public class VideoStorageService : IVideoStorageService
{
    private readonly string _storagePath;

    public VideoStorageService(string storagePath)
    {
        _storagePath = storagePath;
    }

    public async Task CreateVideoFileAsync(IFormFile file, Guid fileId, string fileName, CancellationToken cancellationToken)
    {
        var fileFolder = GetVideoFolderPath(fileId);

        if (!Directory.Exists(fileFolder))
        {
            Directory.CreateDirectory(fileFolder);
        }

        var filePath = GetVideoFilePath(fileId, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream, cancellationToken);
    }

    public string GetVideoFilePath(Guid fileId, string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileId.ToString(), fileName);
        return filePath;
    }

    public string GetVideoFolderPath(Guid fileId)
    {
        var folderPath = Path.Combine(_storagePath, fileId.ToString());
        return folderPath;
    }

    public void DeleteVideoFile(Guid fileId)
    {
        var fileFolder = GetVideoFolderPath(fileId);

        if (Directory.Exists(fileFolder))
        {
            Directory.Delete(fileFolder, true);
        }
    }
}
