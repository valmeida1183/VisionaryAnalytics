using Domain.Entities;
using Infraestructure.VideoAnalyser;
using SharedKernel.Enums;

namespace IntegrationTest.Infrastructure.VideoAnalyzer;

public class FFMpegVideoAnalyzerServiceTests : IDisposable
{
    private readonly FFMpegVideoAnalyzerService _analyzerService;
    private readonly string _testRunDirectory;

    public FFMpegVideoAnalyzerServiceTests()
    {
        _testRunDirectory = Path.Combine(Path.GetTempPath(), "visionary-analytics-tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testRunDirectory);

        _analyzerService = new FFMpegVideoAnalyzerService();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRunDirectory))
        {
            Directory.Delete(_testRunDirectory, true);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ExtractImagesFramesAsync_ShouldCreateCorrectNumberOfFrames()
    {
        // Arrange
        var expectedFrameCount = 24;
        var videoAssetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "video-test.mp4");

        var videoId = Guid.NewGuid();
        var videoProcess = new VideoProcess
        {
            Id = videoId,
            FileName = $"{videoId}.mp4",
            FileExtension = ".mp4",
            FolderPath = "/videos/",
            OriginalName = "video-test.mp4",
            CreatedOn = DateTime.UtcNow,
            Status = ProcessStatus.InProcess,
        };

        // FFMpeg works best when source and destination are in a writeable temp location
        var videoSourceFolder = Path.Combine(_testRunDirectory, videoProcess.Id.ToString());
        Directory.CreateDirectory(videoSourceFolder);

        var videoSourceFilePath = Path.Combine(videoSourceFolder, videoProcess.FileName);
        File.Copy(videoAssetPath, videoSourceFilePath);

        // Act
        var resultFramePaths = await _analyzerService.ExtractImagesFramesAsync(videoSourceFolder, videoProcess);

        // Assert
        Assert.NotNull(resultFramePaths);
        Assert.Equal(expectedFrameCount, resultFramePaths.Count());
        
        var outputFrameFolder = Path.Combine(videoSourceFolder, "Frames");
        var actualFileCount = Directory.GetFiles(outputFrameFolder, "frame_*.png").Length;
        Assert.Equal(expectedFrameCount, actualFileCount);
    }
}
