namespace SharedKernel;
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    //public Error Error { get; }
    public ICollection<Error> Errors { get; } = [];

    //protected Result(bool isSuccess, Error error)
    //{
    //    ValidateResult(isSuccess, error);

    //    IsSuccess = isSuccess;
    //    Errors.Add(error);
    //    //Error = error;
    //}

    protected Result(bool isSuccess, IEnumerable<Error> errors)
    {
        ValidateResult(isSuccess, errors);

        IsSuccess = isSuccess;
        Errors = errors.ToList();
    }

    //public static Result Success() => new(true, Error.None);
    public static Result Success() => new(true, []);
    //public static Result Failure(Error error) => new(false, error);
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors);

    //public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, []);
    public static Result<TValue> Failure<TValue>(IEnumerable<Error> errors) => new(default, false, errors);

    //private static void ValidateResult(bool isSuccess, Error error)
    //{
    //    if (isSuccess && error != Error.None)
    //        throw new InvalidOperationException("A successful result cannot have an error.");

    //    if (!isSuccess && error == Error.None)
    //        throw new InvalidOperationException("A failure result must have an error.");
    //}

    private static void ValidateResult(bool isSuccess, IEnumerable<Error> errors)
    {
        if (isSuccess && errors.Any())
            throw new InvalidOperationException("A successful result cannot have an error.");

        if (!isSuccess && !errors.Any())
            throw new InvalidOperationException("A failure result must have an error.");
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    //public Result(TValue? value, bool isSuccess, Error error)
    //    : base(isSuccess, error)
    //{
    //    _value = value;
    //}

    public Result(TValue? value, bool isSuccess, IEnumerable<Error> errors)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    public TValue? Value => IsSuccess
        ? _value!
        : default;
}
