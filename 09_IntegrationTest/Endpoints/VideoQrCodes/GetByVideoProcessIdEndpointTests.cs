
using Domain.Entities;
using Infraestructure.Database;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Enums;
using System.Net;

namespace IntegrationTest.Endpoints.VideoQrCodes;

public class GetByVideoProcessIdEndpointTests : BaseIntegrationTest
{
    public GetByVideoProcessIdEndpointTests(CustomWebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByVideoProcessId_WhenNoQrCodesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        await using var scope = _services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var videoId = Guid.NewGuid();
        var videoProcess = new VideoProcess
        {
            Id = videoId,
            FileName = "test.mp4",
            FileExtension = ".mp4",
            OriginalName = "test.mp4",
            Status = ProcessStatus.Finished,
            Size = 123
        };

        await dbContext.Set<VideoProcess>().AddAsync(videoProcess);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/qrcodes/{videoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var root = await ParseResponse(response);

        Assert.True(root.GetProperty("isSuccess").GetBoolean());
        var value = root.GetProperty("value");
        Assert.Empty(value.EnumerateArray());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByVideoProcessId_WhenQrCodesExist_ReturnsOkWithQrCodeList()
    {
        // Arrange
        await using var scope = _services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var videoProcess = new VideoProcess
        {
            Id = Guid.NewGuid(),
            FileName = "test.mp4",
            FileExtension = ".mp4",
            OriginalName = "test.mp4",
            Status = ProcessStatus.Finished,
            Size = 123
        };

        var qrCode = new VideoQRCode
        {
            VideoProcessId = videoProcess.Id,
            DataContent = "Hello World",
            TimeStamp = TimeSpan.FromSeconds(1)
        };

        await dbContext.Set<VideoProcess>().AddAsync(videoProcess);
        await dbContext.Set<VideoQRCode>().AddAsync(qrCode);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/qrcodes/{videoProcess.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var root = await ParseResponse(response);

        Assert.True(root.GetProperty("isSuccess").GetBoolean());
        var value = root.GetProperty("value");
        var qrCodeResult = value.EnumerateArray().First();

        Assert.Equal("Hello World", qrCodeResult.GetProperty("dataContent").GetString());
        Assert.Equal(qrCode.Id.ToString(), qrCodeResult.GetProperty("id").GetString());
    }
}
