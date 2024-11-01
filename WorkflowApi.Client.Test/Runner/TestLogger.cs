using System.Diagnostics;
using Newtonsoft.Json;

namespace WorkflowApi.Client.Test.Runner;

public static class TestLogger
{
    public static void LogModelsGenerated<TModel>(IEnumerable<TModel> models, Func<TModel, string> getKey) where TModel : class
    {
        LogInternal("Generated models", models.ToDictionary(getKey, value => value));
    }

    public static void LogUpdateModelsGenerated<TModel>(IEnumerable<TModel> models, Func<TModel, string> getKey) where TModel : class
    {
        LogInternal("Generated update models", models.ToDictionary(getKey, value => value));
    }

    public static void LogAssertingModels<TModel>(TModel expected, TModel? actual) where TModel : class
    {
        LogInternal("Asserting models", new { expected, actual });
    }

    public static void LogAssertingUpdateModels<TModel>(TModel expected, object? actual) where TModel : class
    {
        LogInternal("Asserting update models", new { expected, actual });
    }

    
    public static void LogApiCalled(object? query, object? result)
    {
        LogInternal($"Called API", new { query, result });;
    }
    
    public static void LogEntityGot(object[] keys, object? entity)
    {
        LogInternal($"Got entity with keys [{String.Join(", ", keys)}]", entity ?? "null");
    }

    public static void LogEntitiesCreated<TEntity>(IEnumerable<TEntity> entities, Func<TEntity, string> getKey) where TEntity : class
    {
        LogInternal("Created entities", entities.ToDictionary(getKey, value => value));
    }

    private static void LogInternal(string message, object? properties = null)
    {
        LogSender();

        Console.WriteLine($"  {message}");

        if (properties == null) return;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  " + SerializeValue(properties).Replace("\n", "\n  "));
        Console.ResetColor();
    }

    private static void LogSender()
    {
        var stackTrace = new StackTrace();
        var method = stackTrace.GetFrame(3)?.GetMethod();

        if (method == null) return;

        var fullName = method.DeclaringType == null
            ? method.Name
            : $"{method.DeclaringType.FullName}.{method.Name}";

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{fullName}:");
        Console.ResetColor();
    }

    private static string SerializeValue(object? value)
    {
        return JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        });
    }
}
