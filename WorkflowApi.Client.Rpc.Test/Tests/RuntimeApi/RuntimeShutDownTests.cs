using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.RuntimeApi;

[TestClass]
public class RuntimeShutDownTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldGetRuntimeStatus(TestService service)
    {
        try
        {
            // Arrange

            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());

            var response1 = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

            Assert.IsNotNull(response1);
            Assert.AreEqual(RuntimeRunningStatus.Running, response1.RunningStatus);

            // Act

            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());

            // Assert

            var response2 = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

            Assert.IsNotNull(response2);
            Assert.AreEqual(RuntimeRunningStatus.Stopped, response2.RunningStatus);
        }
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        try
        {
            // Arrange

            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());

            var response1 = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

            Assert.IsNotNull(response1);
            Assert.AreEqual(RuntimeRunningStatus.Running, response1.RunningStatus);

            // Act

            await service.Client.ExclusivePermissions(c => c.RpcRuntime, WorkflowApiOperationId.RpcRuntimeShutDown).WorkflowApiRpcRuntimeShutDownAsync(new());

            // Assert

            var response2 = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

            Assert.IsNotNull(response2);
            Assert.AreEqual(RuntimeRunningStatus.Stopped, response2.RunningStatus);
        }
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange


        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcRuntime, Array.Empty<string>()).WorkflowApiRpcRuntimeShutDownAsync(new ())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
