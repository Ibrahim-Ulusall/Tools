namespace Core.Utilities.Results;
public class SuccessDataResult<T> : DataResult<T>
{
    public SuccessDataResult() : base(default, true) { }
    public SuccessDataResult(T data) : base(data, true) { }
    public SuccessDataResult(string message) : base(message, true, default) { }
    public SuccessDataResult(string message, T data) : base(message, true, data) { }

}
