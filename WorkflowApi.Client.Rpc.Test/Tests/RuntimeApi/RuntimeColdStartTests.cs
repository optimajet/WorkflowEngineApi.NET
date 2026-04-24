using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.RuntimeApi;

[TestClass]
public class RuntimeColdStartTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldGetRuntimeStatus(TestService service)
    {
        try
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());
            
            // Act

            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeColdStartAsync(new());

            // Assert

            var response = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

            Assert.IsNotNull(response);
            Assert.AreEqual(RuntimeRunningStatus.RunningCold, response.RunningStatus);
        }
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        try
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeShutDownAsync(new());
            
            // Act

            var response = await service.Client.WithPermissions(c => c.RpcRuntime, WorkflowApiOperationId.RpcRuntimeColdStart).WorkflowApiRpcRuntimeColdStartAsync(new());

            // Assert

            Assert.IsNotNull(response);
        }
        finally
        {
            await service.Client.RpcRuntime.WorkflowApiRpcRuntimeStartAsync(new());
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange


        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.WithPermissions(c => c.RpcRuntime, Array.Empty<string>()).WorkflowApiRpcRuntimeColdStartAsync(new ())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
