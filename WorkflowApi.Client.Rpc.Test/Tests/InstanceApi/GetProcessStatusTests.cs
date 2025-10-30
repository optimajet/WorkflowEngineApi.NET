using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class GetProcessStatusTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnProcessStatus(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var processId = Guid.NewGuid();
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity")
            .Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));

        // Act

        var idledProcessStatus = ProcessStatusAvailableToSet.Idled;
        await service.Client.RpcInstance.WorkflowApiRpcSetProcessNewStatusAsync(new(processId, idledProcessStatus));
        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new (processId));

        // Assert

        AssertProcessStatus(response, idledProcessStatus);

        // Act

        var finalizedProcessStatus = ProcessStatusAvailableToSet.Finalized;
        await service.Client.RpcInstance.WorkflowApiRpcSetProcessNewStatusAsync(new(processId, finalizedProcessStatus));

        var request = new GetProcessStatusRequest(processId);
        response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(request);

        // Assert

        AssertProcessStatus(response, finalizedProcessStatus);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity")
            .Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        var request = new GetProcessStatusRequest(processId);

        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcInstance, WorkflowApiOperationId.RpcGetProcessStatus)
            .WorkflowApiRpcGetProcessStatusAsync(request);

        // Assert

        Assert.IsNotNull(response.ProcessStatus);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetProcessStatusRequest(Guid.NewGuid());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcGetProcessStatusAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    private static void AssertProcessStatus(GetProcessStatusResponse actual, ProcessStatusAvailableToSet expected)
    {
        Assert.AreEqual(expected.ToString(), actual.ProcessStatus.ToString());
    }
}