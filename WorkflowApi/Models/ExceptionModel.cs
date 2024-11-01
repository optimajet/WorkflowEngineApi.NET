using System.Collections;

namespace WorkflowApi.Models;

internal class ExceptionModel
{
    public ExceptionModel(Exception exception)
    {
        Type = exception.GetType().Name;
        Message = exception.Message;
        Data = exception.Data;
        StackTrace = exception.StackTrace ?? String.Empty;
        Source = exception.Source;
        HelpLink = exception.HelpLink;
        HResult = exception.HResult;
        InnerException = exception.InnerException != null ? new ExceptionModel(exception.InnerException) : null;
    }

    public string Type { get; }
    public string Message { get; }
    public IDictionary Data { get; }
    public string StackTrace { get; }
    public string? Source { get; }
    public string? HelpLink { get; }
    public int HResult { get; }
    public ExceptionModel? InnerException { get; }
}
