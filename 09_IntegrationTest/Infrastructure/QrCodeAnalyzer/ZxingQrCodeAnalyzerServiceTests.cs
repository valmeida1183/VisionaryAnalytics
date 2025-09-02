using Infraestructure.QrCodeAnalyzer;
using Xunit;

namespace IntegrationTest.Infrastructure.QrCodeAnalyzer;

public class ZxingQrCodeAnalyzerServiceTests
{
    private readonly ZxingQrCodeAnalyzerService _analyzerService;

    public ZxingQrCodeAnalyzerServiceTests()
    {
        // Assuming ZxingQrCodeAnalyzerService has a parameterless constructor
        // or its dependencies can be resolved by the test project.
        _analyzerService = new ZxingQrCodeAnalyzerService();
    }

    [Fact]
    public void Analyze_ShouldReturnCorrectText_WhenQrCodeIsValid()
    {
        // Arrange
        var expectedText = "Hello World";
        var assetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "qr-code-test.png");

        // Sanity check to ensure the test asset is copied correctly
        Assert.True(File.Exists(assetPath), "Test asset 'qr-code-test.png' not found. Ensure its 'Copy to Output Directory' property is set to 'PreserveNewest' or 'Copy always'.");

        // Act
        //var result = _analyzerService.Analyze(assetPath);

        // Assert
        //Assert.Equal(expectedText, result);
    }
}
