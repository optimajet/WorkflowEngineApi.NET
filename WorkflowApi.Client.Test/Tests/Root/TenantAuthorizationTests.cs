using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using static WorkflowApi.Client.Test.Runner.MultitenantTestTenants;

namespace WorkflowApi.Client.Test.Tests.Root;

[TestClass]
public class TenantAuthorizationTests
{
    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task AllowedTenantTest(TestService service)
    {
        await service.Client
            .WithPermissions(c => c.RootApi, [WorkflowApiOperationId.Liveness], [service.TenantId])
            .WorkflowApiLivenessAsync();
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task AnotherTenantDeniedTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, [WorkflowApiOperationId.Liveness], [service.TenantId]);
        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = GetAnotherTenantId(service.TenantId);

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task MissingTenantPermissionDeniedTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations().Allow(WorkflowApiOperationId.Liveness));

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task MultipleTenantsAllowedOnlyConfiguredListTest(TestService service)
    {
        var allowedTenant = service.TenantId;
        var allowedTenant2 = GetAnotherTenantId(allowedTenant);
        var deniedTenant = GetTenantIdExcept(allowedTenant, allowedTenant2);

        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .DenyAllTenantsExcept(allowedTenant, allowedTenant2));

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = allowedTenant;
        await api.WorkflowApiLivenessAsync();

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = allowedTenant2;
        await api.WorkflowApiLivenessAsync();

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = deniedTenant;
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task AllTenantsPermissionAllowsEveryConfiguredTenantTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .AllowAllTenants());

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = service.TenantId;
        await api.WorkflowApiLivenessAsync();

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = GetAnotherTenantId(service.TenantId);
        await api.WorkflowApiLivenessAsync();
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task UnknownTenantDeniedTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .AllowAllTenants());

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = "TenantZ";

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task MissingTenantHeaderDeniedTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .AllowAllTenants());

        api.Configuration.DefaultHeaders.Remove(WorkflowApiConstants.TenantIdHeader);

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task InvalidTenantHeaderDeniedTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .AllowAllTenants());

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = new string('a', 129);

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task AllowAllTenantsExceptDeniesConfiguredTenantTest(TestService service)
    {
        var deniedTenant = GetAnotherTenantId(service.TenantId);
        var allowedTenant2 = GetTenantIdExcept(service.TenantId, deniedTenant);
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .AllowAllTenantsExcept(deniedTenant));

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = service.TenantId;
        await api.WorkflowApiLivenessAsync();

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = allowedTenant2;
        await api.WorkflowApiLivenessAsync();

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = deniedTenant;
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task LogicalTenantAllowedWhenExplicitlyAllowedTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .DenyAllTenantsExcept(TenantALogical));

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = TenantALogical;

        await api.WorkflowApiLivenessAsync();
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task LogicalTenantDeniedWithoutExplicitPermissionTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .DenyAllTenantsExcept(TenantA));

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = TenantALogical;

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task WildcardPermissionAllowsRpcBranchTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RpcRuntime, permissions =>
            permissions.DenyAllOperations()
                .Allow("workflow-api.rpc")
                .DenyAllTenantsExcept(service.TenantId));

        var response = await api.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());
        Assert.AreEqual(RuntimeRunningStatus.Stopped, response.RunningStatus);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task ExplicitDenyOverridesWildcardAllowTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RpcRuntime, permissions =>
            permissions.DenyAllOperations()
                .Allow("workflow-api.rpc")
                .Deny(WorkflowApiOperationId.RpcRuntimeGetRunningStatus)
                .DenyAllTenantsExcept(service.TenantId));

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiRpcRuntimeGetRunningStatusAsync(new())
        );

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task HeaderSpoofingDeniedTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.DenyAllOperations()
                .Allow(WorkflowApiOperationId.Liveness)
                .DenyAllTenantsExcept(service.TenantId));

        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = GetAnotherTenantId(service.TenantId);

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await api.WorkflowApiLivenessAsync());
        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task AllPermissionAllowsOperationAndTenantTest(TestService service)
    {
        var api = service.Client.WithPermissions(c => c.RootApi, permissions =>
            permissions.AllowAllOperations().AllowAllTenants());
        api.Configuration.DefaultHeaders[WorkflowApiConstants.TenantIdHeader] = GetAnotherTenantId(service.TenantId);

        await api.WorkflowApiLivenessAsync();
    }

    [ClientTest(HostId.SingleTenantHost)]
    [TestMethod]
    public async Task SingleTenantModeDoesNotRequireTenantClaimTest(TestService service)
    {
        await service.Client
            .WithPermissions(c => c.RootApi, [WorkflowApiOperationId.Liveness], null)
            .WorkflowApiLivenessAsync();
    }

    private static string GetAnotherTenantId(string tenantId)
    {
        return GetTenantIdExcept(tenantId);
    }

    private static string GetTenantIdExcept(params string[] tenantIds)
    {
        var excludedTenantIds = tenantIds.ToHashSet(StringComparer.Ordinal);
        return AllTenantIds.First(tenantId => !excludedTenantIds.Contains(tenantId));
    }

}
