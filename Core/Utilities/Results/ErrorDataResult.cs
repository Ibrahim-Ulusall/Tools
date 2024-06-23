namespace Core.Utilities.Results;

public class ErrorDataResult<T> : DataResult<T>
{
    public ErrorDataResult() : base(default, false) { }
    public ErrorDataResult(T data) : base(data, false) { }
    public ErrorDataResult(string message) : base(message, false, default) { }
    public ErrorDataResult(string message, T data) : base(message, false, data) { }

}
