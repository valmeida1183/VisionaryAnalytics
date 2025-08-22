using Application.Abstractions.MessageBus;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using SharedKernel.Primitives;

namespace Application.VideoProcessResults.Create;
internal sealed class CreateVideoProcessCommandHandler : ICommandHandler<CreateVideoProcessCommand, VideoProcessResult>
{
    private readonly IVideoProcessResultRepository _videoProcessResultRepository;
    private readonly IVideoStorageService _videoStorageService;
    private readonly IMessageEventBusService _messageEventBusService;
    private readonly IValidator<CreateVideoProcessCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideoProcessCommandHandler(
        IVideoProcessResultRepository videoProcessResultRepository,
        IVideoStorageService videoStorageService,
        IMessageEventBusService messageEventBusService,
        IValidator<CreateVideoProcessCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _videoProcessResultRepository = videoProcessResultRepository;
        _videoStorageService = videoStorageService;
        _messageEventBusService = messageEventBusService;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VideoProcessResult>> Handle(CreateVideoProcessCommand command, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Result.Failure<VideoProcessResult>(validationResult.ResultErrors());
        }

        VideoProcessResult videoProcessResult = command;

        await _videoProcessResultRepository.AddAsync(videoProcessResult);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Domain Events?
        await _videoStorageService.CreateVideoFileAsync(command.File, videoProcessResult.FileName, cancellationToken);
        await _messageEventBusService.PublishAsync(videoProcessResult, cancellationToken);

        return Result.Success(videoProcessResult);
    }
}
