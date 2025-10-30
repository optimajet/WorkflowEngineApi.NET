using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.RuntimeApi;

[TestClass]
public class RuntimeGetStatusTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetRuntimeStatus(TestService service)
    {
        // Act

        var response = await service.Client.RpcRuntime.WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

        // Assert

        Assert.IsNotNull(response);
        Assert.AreEqual(RuntimeRunningStatus.Stopped, response.RunningStatus);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange



        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcRuntime, WorkflowApiOperationId.RpcRuntimeGetRunningStatus).WorkflowApiRpcRuntimeGetRunningStatusAsync(new());

        // Assert

        Assert.IsNotNull(response);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange


        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcRuntime, Array.Empty<string>()).WorkflowApiRpcRuntimeGetRunningStatusAsync(new ())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
