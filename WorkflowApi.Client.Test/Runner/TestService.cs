using OptimaJet.Workflow.Api.Options;
using OptimaJet.Workflow.Core;
using OptimaJet.Workflow.Core.Runtime;
using WorkflowApi.Client.Test.Providers;
using WorkflowApi.Client.Test.Repositories;
using WorkflowApi.Client.Test.Repositories.Mongo;
using WorkflowApi.Client.Test.Repositories.Sql;
using WorkflowApi.Exceptions;

namespace WorkflowApi.Client.Test.Runner;

public sealed class TestService
{
    public static async Task<TestService> CreateAsync(Host host, string tenantId)
    {
        var service = new TestService(host, tenantId);
        service._client = await Client.CreateAsync(service);
        return service;
    }
    
    private TestService(Host host, string tenantId)
    {
        Host = host;
        TenantId = tenantId;
        WorkflowRuntime = host.TenantRegistry.Get(tenantId).Runtime;
        TenantOptions = Configuration.AppConfiguration.TenantsConfiguration
            .First(option => option.TenantIds.Contains(TenantId));

        RuleProvider = new TestRuleProvider();
        WorkflowRuntime.WithRuleProvider(RuleProvider);
        
        Repository = TenantOptions.DataProviderId switch
        {
            PersistenceProviderId.Mongo => new MongoRepository(this),
            _ => new SqlRepository(this)
        };
    }

    public Host Host { get; }
    public TestConfiguration Configuration => Host.Configuration;
    public string TenantId { get; }
    public WorkflowRuntime WorkflowRuntime { get; }
    public TestRuleProvider RuleProvider { get; }
    public WorkflowEngineTenantCreationOptions TenantOptions { get; }
    public string DataProviderId => TenantOptions.DataProviderId ?? throw new NotInitializedException(nameof(DataProviderId));
    public IRepository Repository { get; }
    public Client Client => _client ?? throw new NotInitializedException(nameof(Client));
    
    private Client? _client;

    public override int GetHashCode()
    {
        return HashCode.Combine(Host.Id, TenantId);
    }
}