using Domain.ValueObjects;
using Infraestructure.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;

namespace IntegrationTest.Infrastructure.Storage;
public class VideoStorageServiceTests : IDisposable
{
    private readonly VideoStorageService _videoStorageService;
    private readonly string _testDirectory;
    private readonly FileStorageSettings _settings;

    public VideoStorageServiceTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "visionary-analytics-tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        _settings = new FileStorageSettings
        {
            Root = _testDirectory,
            AppFolderName = "app-data",
            VideoFolderName = "videos",
        };

        var options = Options.Create(_settings);
        
        _videoStorageService = new VideoStorageService(options);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateVideoFileAsync_ShouldCreateFileOnDisk()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var videoFileName = $"{fileId}.mp4";
        var fileContent = "This is a test video file.";

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var formFile = new FormFile(stream, 0, stream.Length, "Data", videoFileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        var videoFolderPath = _videoStorageService.GetVideoFolderPath(fileId);

        // Act
        await _videoStorageService.CreateVideoFileAsync(formFile, fileId, videoFileName, CancellationToken.None);

        // Assert
        var expectedFilePath = Path.Combine(videoFolderPath, videoFileName);

        Assert.True(File.Exists(expectedFilePath));

        var savedContent = await File.ReadAllTextAsync(expectedFilePath);
        Assert.Equal(fileContent, savedContent);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void GetVideoFolderPath_ShouldReturnCorrectPath()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var expectedPath = Path.Combine(
            _settings.Root,
            _settings.AppFolderName,
            _settings.VideoFolderName,
            fileId.ToString());

        // Act
        var actualPath = _videoStorageService.GetVideoFolderPath(fileId);

        // Assert
        Assert.Equal(expectedPath, actualPath);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void GetVideoFilePath_ShouldReturnCorrectPath()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var videoFileName = $"{fileId}.mp4";
        var expectedFolderPath = Path.Combine(
            _settings.Root,
            _settings.AppFolderName,
            _settings.VideoFolderName,
            fileId.ToString());

        var expectedFilePath = Path.Combine(expectedFolderPath, videoFileName);

        // Act
        var actualFilePath = _videoStorageService.GetVideoFilePath(fileId, videoFileName);

        // Assert
        Assert.Equal(expectedFilePath, actualFilePath);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void DeleteVideoFolder_ShouldRemoveDirectoryFromDisk()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var videoFolderPath = _videoStorageService.GetVideoFolderPath(fileId);

        Directory.CreateDirectory(videoFolderPath);

        Assert.True(Directory.Exists(videoFolderPath));

        // Act
        _videoStorageService.DeleteVideoFolder(fileId);

        // Assert
        Assert.False(Directory.Exists(videoFolderPath));
    }
}

