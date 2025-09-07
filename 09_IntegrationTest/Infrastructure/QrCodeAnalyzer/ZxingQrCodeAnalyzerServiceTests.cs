using Domain.Entities;
using Infraestructure.QrCodeAnalyzer;
using SharedKernel.Enums;

namespace IntegrationTest.Infrastructure.QrCodeAnalyzer;

public class ZxingQrCodeAnalyzerServiceTests
{
    private readonly ZxingQrCodeAnalyzerService _analyzerService;

    public ZxingQrCodeAnalyzerServiceTests()
    {
        _analyzerService = new ZxingQrCodeAnalyzerService();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Analyze_ShouldReturnCorrectText_WhenQrCodeIsValid()
    {
        // Arrange
        var expectedText = "Hello World";
        var assetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "qr-code-test.png");
        var framesPaths = new List<string> { assetPath };
        var videoProcess = new VideoProcess
        {
            Id = Guid.NewGuid(),
            FileName = "sample.mp4",
            FileExtension = ".mp4",
            FolderPath = "/videos/",
            OriginalName = "sample_original.mp4",
            CreatedOn = DateTime.UtcNow,
            Status = ProcessStatus.InProcess,
        };

        // Sanity check to ensure the test asset is copied correctly
        Assert.True(File.Exists(assetPath), 
            @"Test asset 'qr-code-test.png' not found. 
              Ensure its 'Copy to Output Directory' property is set to 'PreserveNewest' or 'Copy always'.");

        // Act
        var result = await _analyzerService.DecodeQrCodeFromImages(framesPaths, videoProcess);

        // Assert
        Assert.Single(result);
        Assert.Contains(result, r => r.DataContent == expectedText);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DecodeQrCodeFromImages_ShouldReturnEmptyList_WhenImageHasNoQrCode()
    {
        // Arrange
        var assetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "no-qr-code-test.png");
        var framesPaths = new List<string> { assetPath };
        var videoProcess = new VideoProcess
        {
            Id = Guid.NewGuid(),
            FileName = "sample.mp4",
            FileExtension = ".mp4",
            FolderPath = "/videos/",
            OriginalName = "sample_original.mp4",
            CreatedOn = DateTime.UtcNow,
            Status = ProcessStatus.InProcess,
        };

        // Sanity check to ensure the test asset is copied correctly
        Assert.True(File.Exists(assetPath),
            @"Test asset 'no-qr-code-test.png' not found. 
              Ensure its 'Copy to Output Directory' property is set to 'PreserveNewest' or 'Copy always'.");

        // Act
        var result = await _analyzerService.DecodeQrCodeFromImages(framesPaths, videoProcess);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
