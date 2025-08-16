using SharedKernel;

namespace WebApi.Extensions;

public static class ResultExtensions
{
    public static TOut Match<TOut>(
        this Result result, 
        Func<TOut> onSuccess, 
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess
            ? onSuccess()
            : onFailure(result);
    }

    public static TOut Match<TValue, TOut>(
        this Result<TValue> result, 
        Func<Result<TValue>, TOut> onSuccess, 
        Func<Result<TValue>, TOut> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result)
            : onFailure(result);
    }
}
