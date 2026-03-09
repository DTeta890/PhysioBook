namespace PhysioBook.Application.Common;

public sealed record ApiResponse<T>
{
    public T? Data { get; init; }

    public List<string> Errors { get; init; } = [];

    public Dictionary<string, object>? Meta { get; init; }

    public static ApiResponse<T> Success(T data, Dictionary<string, object>? meta = null)
    {
        return new ApiResponse<T> { Data = data, Meta = meta };
    }

    public static ApiResponse<T> Failure(List<string> errors)
    {
        return new ApiResponse<T> { Errors = errors };
    }

    public static ApiResponse<T> Failure(string error)
    {
        return new ApiResponse<T> { Errors = [error] };
    }
}
