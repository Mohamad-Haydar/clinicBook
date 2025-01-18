namespace api.Models.Responce;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; }

    protected Result(bool isSuccess, string Message)
    {
        IsSuccess = isSuccess;
        this.Message = Message;
    }

    public static Result Success(string Message = null) => new Result(true, Message);

    public static Result Failure(string Message) => new Result(false, Message);
}
public class Result<T> : Result
{
    public T Data { get; private set; }

    private Result(bool isSuccess, string Message, T data):base(isSuccess, Message)
    {
        this.Data = data;
    }

    public static Result<T> Success(T? data = default, string Message = null) => new Result<T>(true, Message, data);

    public static Result<T> Failure(string Message) => new Result<T>(false, Message, default);
}