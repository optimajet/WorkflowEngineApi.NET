using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class GetCurrentStateNameTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetCurrentStateName(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Activity2")
            .State("State1")
            .EnableSetState()
            .CreateActivity("Activity3")
            .State("State2")
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var setStateRequest = new SetStateWithoutExecutionRequest {ProcessId = processId, StateName = "State1"};
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithoutExecutionAsync(setStateRequest);

        // Act
        
        var request = new GetCurrentStateNameRequest(processId);
        var response = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateNameAsync(request);

        // Assert

        Assert.AreEqual("State1", response.CurrentStateName);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetCurrentStateName_WhenStateIsNull(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Activity2")
            .State("State1")
            .EnableSetState()
            .CreateActivity("Activity3")
            .State("State2")
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var response = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateNameAsync(new(processId));

        // Assert

        Assert.IsNull(response.CurrentStateName);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act
        
        var request = new GetCurrentStateNameRequest(processId);
        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, WorkflowApiOperationId.RpcGetCurrentStateName).WorkflowApiRpcGetCurrentStateNameAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetCurrentStateNameRequest(Guid.NewGuid());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
            await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcGetCurrentStateNameAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}