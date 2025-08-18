namespace REM.Domain.Dto;

public class Result<T>
{
    public bool Succeeded { get; set; }
    public List<string> Messages { get; set; } = [];
    public T? Data { get; set; }

    public static Result<T> Success()
    {
        return new Result<T> { Succeeded = true };
    }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static Result<T> Success(string message)
    {
        return new Result<T> { Succeeded = true, Messages = [message] };
    }

    public static Result<T> Success(T data, string message)
    {
        return new Result<T>
        {
            Succeeded = true,
            Data = data,
            Messages = [message],
        };
    }

    public static Result<T> Fail()
    {
        return new Result<T> { Succeeded = false };
    }

    public static Result<T> Fail(string message)
    {
        return new Result<T> { Succeeded = false, Messages = [message] };
    }

    public static Result<T> Fail(T data, string message)
    {
        return new Result<T>
        {
            Succeeded = false,
            Data = data,
            Messages = [message],
        };
    }

    public static Result<T> Fail(List<string> messages)
    {
        return new Result<T> { Succeeded = false, Messages = messages };
    }

    public static Result<T> Fail(T data, List<string> messages)
    {
        return new Result<T>
        {
            Succeeded = false,
            Data = data,
            Messages = messages,
        };
    }
}
