using System.Text.Json;
using Newtonsoft.Json;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Parser;
using OptimaJet.Workflow.Core.Runtime;

namespace WorkflowApi.Client.Test.Models;

internal static class Converter
{
    public static string ToCommaSeparatedString(IEnumerable<string> list)
    {
        return String.Join(",", list);
    }

    public static List<string> FromCommaSeparatedString(string? value)
    {
        return value?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList() ?? [];
    }

    public static string ToJsonString(object? value)
    {
        if (value is JsonElement element) return element.GetRawText();
        return JsonConvert.SerializeObject(value);
    }

    public static object? FromJsonObject(string value)
    {
        return JsonConvert.DeserializeObject(value);
    }

    public static List<string> FromJsonStringList(string? value)
    {
        return JsonConvert.DeserializeObject<List<string>>(value ?? "[]") ?? [];
    }

    public static string ToSchemaString(Scheme schema)
    {
        var definition = JsonConvert.DeserializeObject<ProcessDefinition>(ToJsonString(schema));
        return new XmlWorkflowParser().SerializeToString(definition);
    }

    public static Scheme FromSchemaString(string scheme)
    {
        var definition = new XmlWorkflowParser().Parse(new WorkflowRuntime(), scheme);
        return JsonConvert.DeserializeObject<Scheme>(ToJsonString(definition))
               ?? throw new InvalidOperationException("Failed to deserialize scheme from string.");
    }

    public static string? ToTagString(List<string>? tags)
    {
        if (tags == null || tags.Count == 0) return null;
        return JsonConvert.SerializeObject(SimplifyTags(tags));
    }

    public static List<string> SimplifyTags(List<string> tags)
    {
        return tags
            .Distinct()
            .Select(t => t.Trim())
            .Where(t=>!String.IsNullOrEmpty(t))
            .OrderBy(t => t)
            .ToList();
    }
}