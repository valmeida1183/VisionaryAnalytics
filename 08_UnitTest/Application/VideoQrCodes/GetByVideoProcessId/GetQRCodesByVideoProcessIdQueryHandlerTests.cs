using Application.Abstractions.Repositories;
using Application.VideoQrCodes.GetByVideoProcessId;
using Domain.Entities;
using Moq;

namespace UnitTest.Application.VideoQrCodes.GetByVideoProcessId;
public class GetQRCodesByVideoProcessIdQueryHandlerTests
{
    private readonly Mock<IVideoQrCodeRepository> _videoQrCodeRepositoryMock;

    public GetQRCodesByVideoProcessIdQueryHandlerTests()
    {
        _videoQrCodeRepositoryMock = new Mock<IVideoQrCodeRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnQRCodes_WhenVideoProcessIdIsValid()
    {
        // Arrange
        var videoProcessId = Guid.NewGuid();
        var expectedQRCodes = new List<VideoQRCode>
        {
            new() { Id = Guid.NewGuid(), VideoProcessId = videoProcessId, DataContent = "QRCode1" },
            new() { Id = Guid.NewGuid(), VideoProcessId = videoProcessId, DataContent = "QRCode2" }
        };

        _videoQrCodeRepositoryMock
            .Setup(repo => repo.GetByVideoProcessIdAsync(videoProcessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedQRCodes);

        var query = new GetQRCodesByVideoProcessIdQuery(videoProcessId);
        var handler = new GetQRCodesByVideoProcessIdQueryHandler(_videoQrCodeRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedQRCodes, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoVideoQRCodesWasFound()
    {
        // Arrange
        var videoProcessId = Guid.NewGuid();
        var expectedQRCodes = new List<VideoQRCode>();

        _videoQrCodeRepositoryMock
            .Setup(repo => repo.GetByVideoProcessIdAsync(videoProcessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedQRCodes);

        var query = new GetQRCodesByVideoProcessIdQuery(videoProcessId);
        var handler = new GetQRCodesByVideoProcessIdQueryHandler(_videoQrCodeRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedQRCodes, result.Value);
    }
}
