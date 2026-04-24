using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.RuntimeApi;

[TestClass]
public class RuntimeStartTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldGetRuntimeStatus(TestService service)
    {
        await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());

        var response = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

        Assert.IsNotNull(response);
        Assert.AreEqual(RuntimeRunningStatus.Stopped, response.RunningStatus);

        // Act

        await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());

        // Assert

        response = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

        Assert.IsNotNull(response);
        Assert.AreEqual(RuntimeRunningStatus.Running, response.RunningStatus);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());

        var response = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

        Assert.IsNotNull(response);
        Assert.AreEqual(RuntimeRunningStatus.Stopped, response.RunningStatus);
        // Act

        var response1 = await service.Client.WithPermissions(c => c.RpcRuntime, WorkflowApiOperationId.RpcRuntimeStart)
            .WorkflowApiRpcRuntimeStartAsync(new());

        // Assert

        Assert.IsNotNull(response1);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange


        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.WithPermissions(c => c.RpcRuntime, Array.Empty<string>()).WorkflowApiRpcRuntimeStartAsync(new ())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
