using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkflowApi.Client.Test.Runner;

public class ClientTestAttribute : TestCategoryBaseAttribute, ITestDataSource
{
    public ClientTestAttribute(params string[] excludeServices)
    {
        _configurations = TestServiceSet.Configurations
            .Where(configuration => !excludeServices.Contains(configuration.Name))
            .Select(configuration => configuration.Name)
            .ToArray();
    }

    public override IList<string> TestCategories => _configurations.Select(s => s + "ClientTest").ToList();

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        return TestServiceSet.Instance.TestServices
            .Where(service => _configurations.Contains(service.Name))
            .Select(service => new object[] {service});
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var service = data?.First() as TestService;
        var prefix = service?.Name ?? "Unnamed";
        return $"{prefix}{methodInfo.Name}";
    }

    private readonly string[] _configurations;
}
