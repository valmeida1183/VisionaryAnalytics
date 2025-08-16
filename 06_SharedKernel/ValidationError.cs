using SharedKernel.Enums;

namespace SharedKernel;
public sealed record ValidationError : Error
{
    public Error[] Errors { get; }

    public ValidationError(Error[] errors) 
        : base(
            "SharedKernel.ValidationError", 
            "One or more validation errors occurred.", 
            ErrorType.Validation)
    {
        Errors = errors;
    }
}
