using System.Net;
using System.Net.Http.Headers;

namespace IntegrationTest.Endpoints.VideoProcesses;

public class GetStatusByIdEndpointTests : BaseIntegrationTest
{
    public GetStatusByIdEndpointTests(CustomWebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetStatusById_WithValidId_ReturnsOk()
    {
        // Arrange
        // First, create a video process to get a valid ID
        var assetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "video-test.mp4");
        using var videoContent = new StreamContent(File.OpenRead(assetPath));
        videoContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");

        using var formData = new MultipartFormDataContent();
        formData.Add(videoContent, "file", "video-test.mp4");

        var createResponse = await _client.PostAsync("/videos", formData);
        var createJsonDoc = await ParseResponse(createResponse);
        var videoId = createJsonDoc.GetProperty("value").GetProperty("id").GetGuid();

        // Act
        var response = await _client.GetAsync($"/videos/status/{videoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var root = await ParseResponse(response);

        Assert.True(root.GetProperty("isSuccess").GetBoolean());
        Assert.False(root.GetProperty("isFailure").GetBoolean());
        Assert.Empty(root.GetProperty("errors").EnumerateArray());

        var status = root.GetProperty("value").GetString();
        Assert.Equal("Pending", status);
    }
}
