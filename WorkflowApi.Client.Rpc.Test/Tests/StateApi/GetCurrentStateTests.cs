using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class GetCurrentStateTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetCurrentState(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var initialStateName = "InitialState";
        var state1Name = "State";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .State(initialStateName)
            .Initial()
            .CreateActivity("Activity2")
            .State(state1Name)
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new GetCurrentStateRequest(processId);
        var response = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(request);

        // Assert
        
        Assert.IsNotNull(response.CurrentState);
        Assert.AreEqual(schemeCode, response.CurrentState.SchemeCode);
        Assert.AreEqual(initialStateName, response.CurrentState.Name);
        Assert.AreEqual(initialStateName, response.CurrentState.LocalizedName);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetCurrentState_AfterSetState(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var initialStateName = "InitialState";
        var state1Name = "State";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .State(initialStateName)
            .Initial()
            .CreateActivity("Activity2")
            .State(state1Name)
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var setStateRequest = new SetStateWithoutExecutionRequest {ProcessId = processId, StateName = state1Name};
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithoutExecutionAsync(setStateRequest);

        // Act

        var request = new GetCurrentStateRequest(processId);
        var response = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(request);
        
        // Assert
      
        Assert.IsNotNull(response.CurrentState);
        Assert.AreEqual(schemeCode, response.CurrentState.SchemeCode);
        Assert.AreEqual(state1Name, response.CurrentState.Name);
        Assert.AreEqual(state1Name, response.CurrentState.LocalizedName);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetCurrentState_WithCulture(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var initialStateName = "InitialState";

        var localizedStateNameDe = "Zustand1";
        var localizedStateNameFr = "État1";
        var deCulture = new CultureInfo("de-DE");
        var frCulture = new CultureInfo("fr-FR");

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .State(initialStateName)
            .Initial()
            .CreateOrUpdateLocalizationForState(initialStateName, deCulture, localizedStateNameDe)
            .CreateOrUpdateLocalizationForState(initialStateName, frCulture, localizedStateNameFr);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var deRequest = new GetCurrentStateRequest(processId, deCulture.Name);
        var deResponse = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(deRequest);

        var frRequest = new GetCurrentStateRequest(processId, frCulture.Name);
        var frResponse = await service.Client.RpcStates.WorkflowApiRpcGetCurrentStateAsync(frRequest);

        // Assert de-DE

        Assert.IsNotNull(deResponse.CurrentState);
        Assert.AreEqual(schemeCode, deResponse.CurrentState.SchemeCode);
        Assert.AreEqual(initialStateName, deResponse.CurrentState.Name);
        Assert.AreEqual(localizedStateNameDe, deResponse.CurrentState.LocalizedName);

        // Assert fr-FR

        Assert.IsNotNull(frResponse.CurrentState);
        Assert.AreEqual(schemeCode, frResponse.CurrentState.SchemeCode);
        Assert.AreEqual(initialStateName, frResponse.CurrentState.Name);
        Assert.AreEqual(localizedStateNameFr, frResponse.CurrentState.LocalizedName);
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
            .State("State1")
            .Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new GetCurrentStateRequest(processId);
        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, OperationId.RpcGetCurrentState).WorkflowApiRpcGetCurrentStateAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetCurrentStateRequest(Guid.NewGuid());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcGetCurrentStateAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}