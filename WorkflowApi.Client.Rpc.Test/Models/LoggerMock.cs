namespace WorkflowApi.Client.Rpc.Test.Models;

public class LoggerMock : OptimaJet.Workflow.Core.Logging.ILogger
{
    public List<LoggerCall> Calls = new();

    public void Debug(string messageTemplate)
    {
        Calls.Add(new LoggerCall(nameof(Debug), messageTemplate));
    }

    public void Debug(string messageTemplate, params object[] propertyValues)
    {
        Calls.Add(new LoggerCall(nameof(Debug), messageTemplate, propertyValues));
    }

    public void Debug(Exception exception, string messageTemplate)
    {
        Calls.Add(new LoggerCall(nameof(Debug), messageTemplate, null, exception));
    }

    public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Calls.Add(new LoggerCall(nameof(Debug), messageTemplate, propertyValues, exception));
    }

    public void Error(string messageTemplate)
    {
        Calls.Add(new LoggerCall(nameof(Error), messageTemplate));
    }

    public void Error(string messageTemplate, params object[] propertyValues)
    {
        Calls.Add(new LoggerCall(nameof(Error), messageTemplate, propertyValues));
    }

    public void Error(Exception exception, string messageTemplate)
    {
        Calls.Add(new LoggerCall(nameof(Error), messageTemplate, null, exception));
    }

    public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Calls.Add(new LoggerCall(nameof(Error), messageTemplate, propertyValues, exception));
    }

    public void Info(string messageTemplate)
    {
        Calls.Add(new LoggerCall(nameof(Info), messageTemplate));
    }

    public void Info(string messageTemplate, params object[] propertyValues)
    {
        Calls.Add(new LoggerCall(nameof(Info), messageTemplate, propertyValues));
    }

    public void Info(Exception exception, string messageTemplate)
    {
        Calls.Add(new LoggerCall(nameof(Info), messageTemplate, null, exception));
    }

    public void Info(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        Calls.Add(new LoggerCall(nameof(Info), messageTemplate, propertyValues, exception));
    }

    public void Dispose()
    {

    }
}

public record LoggerCall(
    string MethodName,
    string MessageTemplate,
    object[]? PropertyValues = null,
    Exception? Exception = null
);
