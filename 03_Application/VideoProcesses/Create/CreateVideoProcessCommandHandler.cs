using Application.Abstractions.Messaging;
using Application.Extensions;
using Application.Abstractions.Repositories;
using Domain.Entities;
using FluentValidation;
using SharedKernel;
using Application.Abstractions.Storage;

namespace Application.VideoProcesses.Create;
internal sealed class CreateVideoProcessCommandHandler : ICommandHandler<CreateVideoProcessCommand, VideoProcessResult>
{
    private readonly IVideoProcessResultRepository _videoProcessResultRepository;
    private readonly IVideoStorageService _videoStorageService;
    private readonly IValidator<CreateVideoProcessCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideoProcessCommandHandler(
        IVideoProcessResultRepository videoProcessResultRepository,
        IVideoStorageService videoStorageService,
        IValidator<CreateVideoProcessCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _videoProcessResultRepository = videoProcessResultRepository;
        _videoStorageService = videoStorageService;
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

        await _videoStorageService.CreateVideoFileAsync(command.File, videoProcessResult.FileName, cancellationToken);
        
        await _videoProcessResultRepository.AddAsync(videoProcessResult);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish create file event to create the file in the storage

        return Result.Success(videoProcessResult);
    }
}
