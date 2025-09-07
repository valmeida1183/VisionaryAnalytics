using Application.Abstractions.Storage;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infraestructure.Storage;
public class VideoStorageService : IVideoStorageService
{
    private readonly FileStorageSettings _settings;

    public VideoStorageService(IOptions<FileStorageSettings> options)
    {
        _settings = options.Value;
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

    public string GetVideoFolderPath(Guid fileId)
    {
        var folderPath = Path.Combine(
            _settings.Root,
            _settings.AppFolderName,
            _settings.VideoFolderName,
            fileId.ToString());

        return folderPath;
    }

    public string GetVideoFilePath(Guid fileId, string fileName)
    {
        var folderPath = GetVideoFolderPath(fileId);
        var filePath = Path.Combine(folderPath, fileName);

        return filePath;
    }

    public void DeleteVideoFolder(Guid fileId)
    {
        var fileFolder = GetVideoFolderPath(fileId);

        if (Directory.Exists(fileFolder))
        {
            Directory.Delete(fileFolder, true);
        }
    }
}
