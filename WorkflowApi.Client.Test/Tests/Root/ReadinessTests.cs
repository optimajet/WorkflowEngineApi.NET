using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Root;

[TestClass]
public class ReadinessTests
{
    [ClientTest(HostId.SingleTenantHost)]
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
                async () => await service.Client.RootApi.WorkflowApiReadinessAsync()
            );

            Assert.AreEqual(503, exception.ErrorCode);

            var response = JsonConvert.DeserializeObject<ReadinessResponse>(exception.ErrorContent.ToString() ?? "");

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsHealthy);
            Assert.IsNotNull(response.TenantStatuses);

            var stoppedCount = 0;
            string? stoppedTenantId = null;

            foreach (var tenantStatus in response.TenantStatuses)
            {
                Assert.IsNotNull(tenantStatus);
                Assert.IsNotNull(tenantStatus.TenantId);
                Assert.AreEqual(DatabaseStatus.Connected, tenantStatus.DatabaseStatus);
                Assert.IsNull(tenantStatus.DatabaseException);
                Assert.IsNull(tenantStatus.WorkflowRuntimeException);

                if (tenantStatus.WorkflowRuntimeRunningStatus == RuntimeRunningStatus.Stopped)
                {
                    stoppedCount++;
                    stoppedTenantId = tenantStatus.TenantId;
                }
                else
                {
                    Assert.AreNotEqual(RuntimeRunningStatus.Stopped, tenantStatus.WorkflowRuntimeRunningStatus);
                }
            }

            Assert.AreEqual(1, stoppedCount);
            Assert.AreEqual(service.TenantId, stoppedTenantId);

            var expectedMessage = stoppedCount == response.TenantStatuses.Count
                ? "All tenants are unhealthy"
                : "Some tenants are unhealthy";

            Assert.AreEqual(expectedMessage, response.Message);
        }
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());
        }
    }

    [ClientTest(HostId.SingleTenantHost)]
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
                async () => await service.Client.WithPermissions(c => c.RootApi, [WorkflowApiOperationId.Readiness]).WorkflowApiReadinessAsync()
            );

            Assert.AreEqual(503, exception.ErrorCode);
        }
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());
        }
    }
    
    [ClientTest(HostId.SingleTenantHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.WithPermissions(c => c.RootApi, Array.Empty<string>()).WorkflowApiReadinessAsync()
        );
        
        Assert.AreEqual(403, exception.ErrorCode);
    }
}
