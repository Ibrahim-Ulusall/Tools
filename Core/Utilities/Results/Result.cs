namespace Core.Utilities.Results;
public class Result(bool success) : IResult
{
    public string Message { get; set; }
    public bool Success { get; set; } = success;

    public Result(string message, bool success) : this(success)
    {
        Message = message;
    }
}
