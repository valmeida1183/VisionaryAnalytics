using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using FluentValidation;
using SharedKernel;

namespace Application.VideoProcesses.Create;
internal sealed class CreateVideoProcessCommandHandler : ICommandHandler<CreateVideoProcessCommand, Guid>
{
    private readonly IVideoProcessResultRepository _videoProcessResultRepository;
    private readonly IValidator<CreateVideoProcessCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideoProcessCommandHandler(
        IVideoProcessResultRepository videoProcessResultRepository,
        IValidator<CreateVideoProcessCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _videoProcessResultRepository = videoProcessResultRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateVideoProcessCommand command, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(validationResult.ResultErrors());
        }

        VideoProcessResult videoProcessResult = command;
        
        await _videoProcessResultRepository.AddAsync(videoProcessResult);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish create file event

        return Result.Success(videoProcessResult.Id);
    }
}
