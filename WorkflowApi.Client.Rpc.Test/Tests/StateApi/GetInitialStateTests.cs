using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class GetInitialStateTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetInitialState(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var stateName1 = "State1";
        var localizedStateName1Fr = "État1";
        var frCulture = new CultureInfo("fr-FR");

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .State(stateName1)
            .Initial()
            .CreateOrUpdateLocalizationForState(stateName1, frCulture, localizedStateName1Fr);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new GetInitialStateRequest(schemeCode, frCulture.Name);
        var response = await service.Client.RpcStates.WorkflowApiRpcGetInitialStateAsync(request);

        // Assert
        
        Assert.AreEqual(stateName1, response.InitialState.Name);
        Assert.AreEqual(localizedStateName1Fr, response.InitialState.LocalizedName);
        Assert.AreEqual(schemeCode, response.InitialState.SchemeCode);
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

        var request = new GetInitialStateRequest(schemeCode);
        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, WorkflowApiOperationId.RpcGetInitialState).WorkflowApiRpcGetInitialStateAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetInitialStateRequest("Test");

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcGetInitialStateAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}