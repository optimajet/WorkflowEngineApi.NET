using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;

namespace WorkflowApi.Client.Test.Providers;

public class TestRuleProvider : IWorkflowRuleProvider
{
    public List<Rule> Rules { get; set; } = [];

    public string CreateRule(
        Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task<bool>> checkFunc,
        Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task<IEnumerable<string>>> getIdentitiesFunc
    )
    {
        var name = Guid.NewGuid().ToString();

        Rules.Add(new Rule(name, checkFunc, getIdentitiesFunc));

        return name;
    }

    public List<string> GetRules(string schemeCode, NamesSearchType namesSearchType)
    {
        return Rules.Select(r => r.Name).ToList();
    }

    public bool Check(ProcessInstance processInstance, WorkflowRuntime runtime, string identityId, string ruleName,
        string parameter)
    {
        // All rules are async in this test provider
        throw new NotImplementedException();
    }

    public Task<bool> CheckAsync(ProcessInstance processInstance, WorkflowRuntime runtime, string identityId, string ruleName,
        string parameter, CancellationToken token)
    {
        var rule = Rules.FirstOrDefault(r => r.Name == ruleName);

        if (rule == null)
        {
            throw new NotImplementedException($"Rule '{ruleName}' not found.");
        }

        return rule.CheckFunc(processInstance, runtime, identityId, token);
    }

    public IEnumerable<string> GetIdentities(ProcessInstance processInstance, WorkflowRuntime runtime, string ruleName, string parameter)
    {
        // All rules are async in this test provider
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetIdentitiesAsync(ProcessInstance processInstance, WorkflowRuntime runtime, string ruleName, string parameter, CancellationToken token)
    {
        var rule = Rules.FirstOrDefault(r => r.Name == ruleName);

        if (rule == null)
        {
            throw new NotImplementedException($"Rule '{ruleName}' not found.");
        }

        return rule.GetIdentitiesFunc(processInstance, runtime, ruleName, token);
    }

    public bool IsCheckAsync(string ruleName, string schemeCode)
    {
        // All rules are async in this test provider
        return true;
    }

    public bool IsGetIdentitiesAsync(string ruleName, string schemeCode)
    {
        // All rules are async in this test provider
        return true;
    }
}

public record Rule(
    string Name,
    Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task<bool>> CheckFunc,
    Func<ProcessInstance, WorkflowRuntime, string, CancellationToken, Task<IEnumerable<string>>> GetIdentitiesFunc
);
