using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class GetCurrentActivityNameTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetCurrentActivityName(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Activity2")
            .CreateActivity("Activity3")
            .EnableSetState()
            .State("State3");

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new GetCurrentActivityNameRequest(processId);
        var response = await service.Client.RpcStates.WorkflowApiRpcGetCurrentActivityNameAsync(request);

        // Assert

        Assert.AreEqual("Activity1", response.CurrentActivityName);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetCurrentActivityName_AfterSetState(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Activity2")
            .CreateActivity("Activity3")
            .EnableSetState()
            .State("State3");

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var setStateRequest = new SetStateWithoutExecutionRequest {ProcessId = processId, StateName = "State3"};
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithoutExecutionAsync(setStateRequest);
        
        // Act
        
        var request = new GetCurrentActivityNameRequest(processId);
        var response = await service.Client.RpcStates.WorkflowApiRpcGetCurrentActivityNameAsync(request);

        // Assert

        Assert.AreEqual("Activity3", response.CurrentActivityName);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new GetCurrentActivityNameRequest(processId);
        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, WorkflowApiOperationId.RpcGetCurrentActivityName).WorkflowApiRpcGetCurrentActivityNameAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetCurrentActivityNameRequest(Guid.NewGuid());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcGetCurrentActivityNameAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}