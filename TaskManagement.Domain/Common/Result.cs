using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error Error { get; }
}

public class Result<T> : Result
{
    private readonly T? _value;

    private Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
        => _value = value;

    public T Value => _value!;

    public static implicit operator Result<T>(T value) => new(value, true, Error.None);
    public static implicit operator Result<T>(Error error) => new(default, false, error);
}