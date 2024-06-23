namespace Core.Utilities.Results;
public class DataResult<T> : Result, IDataResult<T>
{
    public T Data { get; set; }

    public DataResult(T data,bool success):base(success)
    {
        Data = data;
    }
    public DataResult(string message,bool success,T data):base(message,success)
    {
        Data = data;
    }

}
