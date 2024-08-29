using System.Text.Json.Serialization;

namespace WorkflowApi.Models;

internal class Error
{
    public static Error Create(Exception exception) => new(exception);

    private Error(Exception exception)
    {
        Code = ErrorCode.Exception;
        Message = exception.Message;
        Exception = new ExceptionModel(exception);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorCode Code { get; }
    public string Message { get; }
    public ExceptionModel? Exception { get; }
}

enum ErrorCode
{
    Unknown,
    Exception,
}
