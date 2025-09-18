using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkflowApi.Client.Test.Runner;

public class ClientTestAttribute : TestCategoryBaseAttribute, ITestDataSource
{
    public ClientTestAttribute(string hostId) : this([hostId]) { }
    
    public ClientTestAttribute(string[] hostIds)
    {
        _hosts = TestHostRegister.Instance.Hosts
            .Where(host => hostIds.Contains(host.Id))
            .ToArray();
        
        _providerIds = _hosts
            .SelectMany(host => host.Configuration.AppConfiguration.TenantsConfiguration.Select(tenant => tenant.DataProviderId).Distinct())
            .Distinct()
            .Where(id => id != null)
            .Cast<string>()
            .ToArray();
    }

    public string[] ExcludeProviders { get; set; } = [];
    public override IList<string> TestCategories => _hosts.Select(host => host.Id).Union(FilteredProviderIds).ToArray();

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        return _hosts.SelectMany(host => host.Services)
            .Where(service => FilteredProviderIds.Contains(service.TenantOptions.DataProviderId))
            .Select(service => new object[] { service });
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var service = data?.First() as TestService ?? throw new ArgumentNullException(nameof(data));
        return $"{service.Host.Id}_{service.TenantId}_{methodInfo.Name}";
    }

    private string[] FilteredProviderIds => _providerIds.Except(ExcludeProviders).ToArray();
    
    private readonly Host[] _hosts;
    private readonly string[] _providerIds;
}
