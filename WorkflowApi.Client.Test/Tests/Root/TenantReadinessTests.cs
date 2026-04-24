using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Root;

[TestClass]
public class TenantReadinessTests
{
    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task ExecuteTest(TestService service)
    {
        try
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());

            var runtimeStatus = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

            Assert.IsNotNull(runtimeStatus);
            Assert.AreEqual(RuntimeRunningStatus.Stopped, runtimeStatus.RunningStatus);

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
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());
        }
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task PermissionAllowedTest(TestService service)
    {
        try
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());

            var runtimeStatus = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

            Assert.IsNotNull(runtimeStatus);
            Assert.AreEqual(RuntimeRunningStatus.Stopped, runtimeStatus.RunningStatus);

            var exception = await Assert.ThrowsExceptionAsync<ApiException>(
                async () => await service.Client.WithPermissions(c => c.RootApi, [WorkflowApiOperationId.TenantReadiness]).WorkflowApiTenantReadinessAsync()
            );

            Assert.AreEqual(503, exception.ErrorCode);
        }
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());
        }
    }
    
    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.WithPermissions(c => c.RootApi, Array.Empty<string>()).WorkflowApiTenantReadinessAsync()
        );
        
        Assert.AreEqual(403, exception.ErrorCode);
    }
}
