using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.Root;

[TestClass]
public class ReadinessTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.RootApi.WorkflowApiReadinessAsync()
        );

        Assert.AreEqual(503, exception.ErrorCode);

        var response = JsonConvert.DeserializeObject<ReadinessResponse>(exception.ErrorContent.ToString() ?? "");

        Assert.IsNotNull(response);
        Assert.IsFalse(response.IsHealthy);
        Assert.AreEqual("All tenants are unhealthy", response.Message);
        Assert.IsNotNull(response.TenantStatuses);

        foreach (var tenantStatus in response.TenantStatuses)
        {
            Assert.IsNotNull(tenantStatus);
            Assert.IsNotNull(tenantStatus.TenantId);
            Assert.IsFalse(tenantStatus.IsHealthy);
            Assert.AreEqual(DatabaseStatus.Connected, tenantStatus.DatabaseStatus);
            Assert.IsNull(tenantStatus.DatabaseException);
            Assert.AreEqual(RuntimeRunningStatus.Stopped, tenantStatus.WorkflowRuntimeRunningStatus);
            Assert.IsNull(tenantStatus.WorkflowRuntimeException);
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RootApi, [WorkflowApiOperationId.Readiness]).WorkflowApiReadinessAsync()
        );

        Assert.AreEqual(503, exception.ErrorCode);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RootApi, Array.Empty<string>()).WorkflowApiReadinessAsync()
        );
        
        Assert.AreEqual(403, exception.ErrorCode);
    }
}
