using Application.Abstractions.MessageBus;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using SharedKernel.Primitives;

namespace Application.VideoProcesses.Create;
internal sealed class CreateVideoProcessCommandHandler : ICommandHandler<CreateVideoProcessCommand, VideoProcess>
{
    private readonly IVideoProcessRepository _videoProcessRepository;
    private readonly IVideoStorageService _videoStorageService;
    private readonly IMessageEventBusService _messageEventBusService;
    private readonly IValidator<CreateVideoProcessCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideoProcessCommandHandler(
        IVideoProcessRepository videoProcessRepository,
        IVideoStorageService videoStorageService,
        IMessageEventBusService messageEventBusService,
        IValidator<CreateVideoProcessCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _videoProcessRepository = videoProcessRepository;
        _videoStorageService = videoStorageService;
        _messageEventBusService = messageEventBusService;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VideoProcess>> Handle(CreateVideoProcessCommand command, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Result.Failure<VideoProcess>(validationResult.ResultErrors());
        }

        VideoProcess videoProcess = command;
        videoProcess.FolderPath = _videoStorageService.GetVideoFolderPath(videoProcess.Id);

        await _videoProcessRepository.AddAsync(videoProcess);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Domain Events?
        await _videoStorageService.CreateVideoFileAsync(command.File, videoProcess.Id, videoProcess.FileName, cancellationToken);
        await _messageEventBusService.PublishAsync(new VideoProcessCreatedIntegrationEvent(videoProcess.Id), cancellationToken);

        return Result.Success(videoProcess);
    }
}
