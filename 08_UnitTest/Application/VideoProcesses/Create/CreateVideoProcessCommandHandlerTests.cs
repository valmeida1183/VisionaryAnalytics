using Application.Abstractions.MessageBus;
using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.VideoProcesses.Create;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTest.Application.VideoProcesses.Create;
public class CreateVideoProcessCommandHandlerTests
{
    private readonly Mock<IVideoProcessRepository> _videoProcessRepositoryMock;
    private readonly Mock<IVideoStorageService> _videoStorageServiceMock;
    private readonly Mock<IMessageEventBusService> _messageEventBusServiceMock;
    private readonly Mock<IValidator<CreateVideoProcessCommand>> _validatorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateVideoProcessCommandHandlerTests()
    {
        _videoProcessRepositoryMock = new Mock<IVideoProcessRepository>();
        _videoStorageServiceMock = new Mock<IVideoStorageService>();
        _messageEventBusServiceMock = new Mock<IMessageEventBusService>();
        _validatorMock = new Mock<IValidator<CreateVideoProcessCommand>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_ShouldCreateVideoProcess_WhenCommandIsValid()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("sample.mp4");
        fileMock.Setup(f => f.Length).Returns(100000);

        var command = new CreateVideoProcessCommand(fileMock.Object);

        _validatorMock
            .Setup(v => v.Validate(command))
            .Returns(new ValidationResult());

        _videoStorageServiceMock
            .Setup(svc => svc.GetVideoFolderPath(It.IsAny<Guid>()))
            .Returns("/videos/");

        _videoStorageServiceMock
            .Setup(svc => svc.CreateVideoFileAsync(
                It.IsAny<IFormFile>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _videoProcessRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<VideoProcess>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _messageEventBusServiceMock
            .Setup(bus => bus.PublishAsync(It.IsAny<VideoProcessCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateVideoProcessCommandHandler(
            _videoProcessRepositoryMock.Object,
            _videoStorageServiceMock.Object,
            _messageEventBusServiceMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _videoProcessRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<VideoProcess>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _videoStorageServiceMock.Verify(svc => svc.CreateVideoFileAsync(command.File, It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _messageEventBusServiceMock.Verify(bus => bus.PublishAsync(It.IsAny<VideoProcessCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenCommandIsInvalid()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("sample.mp4");
        fileMock.Setup(f => f.Length).Returns(100000);

        var command = new CreateVideoProcessCommand(fileMock.Object);

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("File", "File is required.")
        };

        _validatorMock
            .Setup(v => v.Validate(command))
            .Returns(new ValidationResult(validationFailures));

        var handler = new CreateVideoProcessCommandHandler(
            _videoProcessRepositoryMock.Object,
            _videoStorageServiceMock.Object,
            _messageEventBusServiceMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.Description == "File is required.");
        _videoProcessRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<VideoProcess>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _videoStorageServiceMock.Verify(svc => svc.CreateVideoFileAsync(It.IsAny<IFormFile>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _messageEventBusServiceMock.Verify(bus => bus.PublishAsync(It.IsAny<VideoProcessCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
