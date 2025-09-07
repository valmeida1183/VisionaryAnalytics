using Application.Abstractions.Repositories;
using Application.VideoProcesses.GetById;
using Domain.Entities;
using Moq;
using SharedKernel.Enums;

namespace UnitTest.Application.VideoProcesses.GetStatusById;
public class GetVideoProcessStatusByIdQueryHandlerTests
{
    private readonly Mock<IVideoProcessRepository> _videoProcessStatusRepositoryMock;

    public GetVideoProcessStatusByIdQueryHandlerTests()
    {
        _videoProcessStatusRepositoryMock = new Mock<IVideoProcessRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnVideoProcessStatus_WhenVideoProcessIdIsValid()
    {
        // Arrange
        var videoProcessId = Guid.NewGuid();
        var processStatus = ProcessStatus.InProcess;
        var videoProcess = new VideoProcess
        {
            Id = videoProcessId,
            FileName = "sample.mp4",
            FileExtension = ".mp4",
            FolderPath = "/videos/",
            OriginalName = "sample_original.mp4",
            CreatedOn = DateTime.UtcNow,
            Status = processStatus
        };

        _videoProcessStatusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(videoProcessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoProcess);

        var query = new GetVideoProcessStatusByIdQuery(videoProcessId);
        var handler = new GetVideoProcessStatusByIdQueryHandler(_videoProcessStatusRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(processStatus.ToString(), result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenVideoProcessIdIsInvalid()
    {
        // Arrange
        var videoProcessId = Guid.NewGuid();

        _videoProcessStatusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(videoProcessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: null);

        var query = new GetVideoProcessStatusByIdQuery(videoProcessId);
        var handler = new GetVideoProcessStatusByIdQueryHandler(_videoProcessStatusRepositoryMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == "NotFound");
    }
}
