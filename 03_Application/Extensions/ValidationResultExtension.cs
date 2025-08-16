using FluentValidation.Results;
using SharedKernel;

namespace Application.Extensions;
internal static class ValidationResultExtension
{
    public static IEnumerable<Error> ResultErrors(this ValidationResult validationResult)
    {
       return validationResult.Errors
                .Select(vf => Error.Failure(vf.ErrorCode, vf.ErrorMessage))
                .ToList();
    }
}
