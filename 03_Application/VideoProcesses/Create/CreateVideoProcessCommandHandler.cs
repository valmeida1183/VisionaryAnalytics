using Application.Abstractions.Messaging;
using Application.Extensions;
using FluentValidation;
using SharedKernel;

namespace Application.VideoProcesses.Create;
internal sealed class CreateVideoProcessCommandHandler(
    IValidator<CreateVideoProcessCommand> validator) 
    : ICommandHandler<CreateVideoProcessCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateVideoProcessCommand command, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(validationResult.ResultErrors());
        }

        return Result.Success(Guid.NewGuid());
    }
}
