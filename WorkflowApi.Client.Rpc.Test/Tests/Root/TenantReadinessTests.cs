using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.Root;

[TestClass]
public class TenantReadinessTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.RootApi.WorkflowApiTenantReadinessWithHttpInfoAsync()
        );

        Assert.AreEqual(503, exception.ErrorCode);

        var response = JsonConvert.DeserializeObject<TenantReadinessResponse>(exception.ErrorContent.ToString() ?? "");

        Assert.IsNotNull(response?.TenantStatus);

        var tenantStatus = response.TenantStatus;
        Assert.IsNotNull(tenantStatus);
        Assert.AreEqual(service.TenantId, tenantStatus.TenantId);
        Assert.IsFalse(tenantStatus.IsHealthy);
        Assert.AreEqual(DatabaseStatus.Connected, tenantStatus.DatabaseStatus);
        Assert.IsNull(tenantStatus.DatabaseException);
        Assert.AreEqual(RuntimeRunningStatus.Stopped, tenantStatus.WorkflowRuntimeRunningStatus);
        Assert.IsNull(tenantStatus.WorkflowRuntimeException);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RootApi, [WorkflowApiOperationId.TenantReadiness]).WorkflowApiTenantReadinessAsync()
        );

        Assert.AreEqual(503, exception.ErrorCode);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RootApi, Array.Empty<string>()).WorkflowApiTenantReadinessAsync()
        );
        
        Assert.AreEqual(403, exception.ErrorCode);
    }
}
