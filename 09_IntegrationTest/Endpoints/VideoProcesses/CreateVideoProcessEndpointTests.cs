using Application.VideoProcesses.Create;
using System.Net;
using System.Net.Http.Headers;

namespace IntegrationTest.Endpoints.VideoProcesses;

public class CreateVideoProcessEndpointTests : BaseIntegrationTest
{
    public CreateVideoProcessEndpointTests(CustomWebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateVideoProcess_WithValidVideo_ReturnsOkAndPublishesIntegrationEvent()
    {
        // Arrange
        var assetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "video-test.mp4");
        using var videoContent = new StreamContent(File.OpenRead(assetPath));
        videoContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");

        using var formData = new MultipartFormDataContent();
        formData.Add(videoContent, "file", "video-test.mp4");

        // Act
        var response = await _client.PostAsync("/videos", formData);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var root = await ParseResponse(response);

        Assert.True(root.GetProperty("isSuccess").GetBoolean());
        Assert.False(root.GetProperty("isFailure").GetBoolean());
        Assert.Empty(root.GetProperty("errors").EnumerateArray());

        var value = root.GetProperty("value");
        Assert.Equal("video-test.mp4", value.GetProperty("originalName").GetString());

        var videoId = value.GetProperty("id").GetGuid();
        Assert.NotEqual(Guid.Empty, videoId);

        Assert.True(await _harness.Published
            .Any<VideoProcessCreatedIntegrationEvent>(x => x.Context.Message.Id == videoId));
    }
}

