using Application.Abstractions.QrCodeAnalyzer;
using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.Abstractions.VideoAnalyser;
using Application.VideoProcesses.Analyze;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using SharedKernel.Enums;

namespace UnitTest.Application.VideoProcesses.Analyze;
public class AnalyzeVideoProcessCommandHandlerTests
{
    private readonly Mock<IVideoProcessRepository> _videoProcessRepositoryMock;
    private readonly Mock<IVideoQrCodeRepository> _videoQrCodeRepositoryMock;
    private readonly Mock<IVideoStorageService> _videoStorageServiceMock;
    private readonly Mock<IVideoFrameAnalyzerService> _videoFrameAnalyserServiceMock;
    private readonly Mock<IQrCodeAnalyzerService> _qrCodeAnalyzerServiceMock;
    private readonly Mock<IValidator<AnalyzeVideoProcessCommand>> _validatorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AnalyzeVideoProcessCommandHandlerTests()
    {
        _videoProcessRepositoryMock = new Mock<IVideoProcessRepository>();
        _videoQrCodeRepositoryMock = new Mock<IVideoQrCodeRepository>();
        _videoStorageServiceMock = new Mock<IVideoStorageService>();
        _videoFrameAnalyserServiceMock = new Mock<IVideoFrameAnalyzerService>();
        _qrCodeAnalyzerServiceMock = new Mock<IQrCodeAnalyzerService>();
        _validatorMock = new Mock<IValidator<AnalyzeVideoProcessCommand>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_ShouldAnalyzeVideoProcess_WhenCommandIsValid()
    {
        // Arrange
        var videoProcess = new VideoProcess
        {
            Id = Guid.NewGuid(),
            FileName = "sample.mp4",
            FileExtension = ".mp4",
            FolderPath = "/videos/",
            OriginalName = "sample_original.mp4",
            CreatedOn = DateTime.UtcNow,
            Status = ProcessStatus.Pending
        };

        var videQRCodes = new List<VideoQRCode>
        {
            new() { Id = Guid.NewGuid(), VideoProcessId = videoProcess.Id, DataContent = "QRCode1" },
            new() { Id = Guid.NewGuid(), VideoProcessId = videoProcess.Id, DataContent = "QRCode2" }
        };

        var framesPaths = new List<string> { "/videos/frames/000001.png", "/videos/frames/000002.png" };

        var command = new AnalyzeVideoProcessCommand(videoProcess);

        _validatorMock
            .Setup(v => v.Validate(command))
            .Returns(new FluentValidation.Results.ValidationResult());

        _videoStorageServiceMock
            .Setup(svc => svc.DeleteVideoFolder(It.IsAny<Guid>()));

        _videoFrameAnalyserServiceMock
            .Setup(svc => svc.ExtractImagesFramesAsync(
                It.IsAny<string>(),
                It.IsAny<VideoProcess>()))
            .ReturnsAsync(framesPaths);

        _qrCodeAnalyzerServiceMock
            .Setup(svc => svc.DecodeQrCodeFromImages(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<VideoProcess>()))
            .ReturnsAsync(videQRCodes);

        _videoQrCodeRepositoryMock
            .Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<VideoQRCode>>()))
            .Returns(Task.CompletedTask);

        _videoProcessRepositoryMock
            .Setup(repo => repo.Update(It.IsAny<VideoProcess>()));

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new AnalyzeVideoProcessCommandHandler(
            _videoProcessRepositoryMock.Object,
            _videoQrCodeRepositoryMock.Object,
            _videoStorageServiceMock.Object,
            _videoFrameAnalyserServiceMock.Object,
            _qrCodeAnalyzerServiceMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _videoProcessRepositoryMock.Verify(
            repo => repo.Update(It.Is<VideoProcess>(vp => vp.Status == ProcessStatus.Finished)), Times.Exactly(2));
        _videoQrCodeRepositoryMock.Verify(
            repo => repo.AddRangeAsync(It.IsAny<IEnumerable<VideoQRCode>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCommandIsInvalid()
    {
        // Arrange
        var videoProcess = new VideoProcess
        {
            Id = Guid.NewGuid(),
            FileName = "sample.mp4",
            FileExtension = ".mp4",
            FolderPath = "/videos/",
            OriginalName = "sample_original.mp4",
            CreatedOn = DateTime.UtcNow,
            Status = ProcessStatus.Pending
        };

        var command = new AnalyzeVideoProcessCommand(videoProcess);

        var validationErrors = new List<ValidationFailure>
        {
            new("VideoProcess", "VideoProcess is required.")
        };

        _validatorMock
            .Setup(v => v.Validate(command))
            .Returns(new ValidationResult(validationErrors));

        var handler = new AnalyzeVideoProcessCommandHandler(
            _videoProcessRepositoryMock.Object,
            _videoQrCodeRepositoryMock.Object,
            _videoStorageServiceMock.Object,
            _videoFrameAnalyserServiceMock.Object,
            _qrCodeAnalyzerServiceMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Description == "VideoProcess is required.");
        _videoProcessRepositoryMock.Verify(repo => repo.Update(It.IsAny<VideoProcess>()), Times.Never);
        _videoQrCodeRepositoryMock.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<VideoQRCode>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        // Arrange
        var videoProcess = new VideoProcess
        {
            Id = Guid.NewGuid(),
            FileName = "sample.mp4",
            FileExtension = ".mp4",
            FolderPath = "/videos/",
            OriginalName = "sample_original.mp4",
            CreatedOn = DateTime.UtcNow,
            Status = ProcessStatus.Pending
        };

        var command = new AnalyzeVideoProcessCommand(videoProcess);

        _validatorMock
            .Setup(v => v.Validate(command))
            .Returns(new ValidationResult());

        _videoFrameAnalyserServiceMock
            .Setup(svc => svc.ExtractImagesFramesAsync(
                It.IsAny<string>(),
                It.IsAny<VideoProcess>()))
            .ThrowsAsync(new Exception("Frame extraction error"));

        _videoProcessRepositoryMock
            .Setup(repo => repo.Update(It.IsAny<VideoProcess>()));

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new AnalyzeVideoProcessCommandHandler(
            _videoProcessRepositoryMock.Object,
            _videoQrCodeRepositoryMock.Object,
            _videoStorageServiceMock.Object,
            _videoFrameAnalyserServiceMock.Object,
            _qrCodeAnalyzerServiceMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Code == "VideoProcess.AnalyzeError");
        _videoProcessRepositoryMock.Verify(
            repo => repo.Update(It.Is<VideoProcess>(vp => vp.Status == ProcessStatus.Failure)), Times.Exactly(2));
        _videoQrCodeRepositoryMock.Verify(
            repo => repo.AddRangeAsync(It.IsAny<IEnumerable<VideoQRCode>>()), Times.Never);
    }
}
