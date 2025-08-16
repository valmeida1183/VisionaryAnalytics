using Application.Abstractions.Messaging;
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
            var errors = validationResult.Errors
                .Select(vf => Error.Failure(vf.ErrorCode, vf.ErrorMessage)).ToArray();

            var validationError = new ValidationError(errors);
            
            return Result.Failure<Guid>(validationError);
        }

        return Result.Success(Guid.NewGuid());
    }
}
