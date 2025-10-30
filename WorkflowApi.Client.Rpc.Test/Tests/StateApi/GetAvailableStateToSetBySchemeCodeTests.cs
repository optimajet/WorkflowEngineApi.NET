using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class GetAvailableStateToSetBySchemeCodeTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetAvailableStatesToSetBySchemeCode(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var stateName1 = "State1";
        var stateName3 = "State3";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Test1")
            .State(stateName1)
            .EnableSetState()
            .CreateActivity("Test2")
            .DisableSetState()
            .CreateActivity("Test3")
            .State(stateName3)
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var result = await service.Client.RpcStates.WorkflowApiRpcGetAvailableStatesToSetBySchemeCodeAsync(new(schemeCode));

        // Assert

        Assert.IsNotNull(result);
        Assert.AreEqual(result.AvailableStates.Count, 2);

        var availableState1 = result.AvailableStates.FirstOrDefault(x => x.Name == stateName1);
        Assert.IsNotNull(availableState1);
        Assert.AreEqual(availableState1.SchemeCode, schemeCode);
        Assert.AreEqual(availableState1.LocalizedName, stateName1);

        var availableState3 = result.AvailableStates.FirstOrDefault(x => x.Name == stateName3);
        Assert.IsNotNull(availableState3);
        Assert.AreEqual(availableState3.SchemeCode, schemeCode);
        Assert.AreEqual(availableState3.LocalizedName, stateName3);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetAvailableStatesToSetBySchemeCode_WithCulture(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();
        var stateName1 = "State1";
        var stateName3 = "State3";
        var localizedStateName1Fr = "État1";
        var localizedStateName3Fr = "État3";
        var frCulture = new CultureInfo("fr-FR");

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Test1")
            .State(stateName1)
            .EnableSetState()
            .CreateOrUpdateLocalizationForState(stateName1, frCulture, localizedStateName1Fr)
            .CreateActivity("Test2")
            .DisableSetState()
            .CreateActivity("Test3")
            .State(stateName3)
            .EnableSetState()
            .CreateOrUpdateLocalizationForState(stateName3, frCulture, localizedStateName3Fr);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        // Act

        var resultFr = await service.Client.RpcStates.WorkflowApiRpcGetAvailableStatesToSetBySchemeCodeAsync(new (schemeCode, frCulture.Name));

        // Assert fr-FR
        
        Assert.IsNotNull(resultFr);
        Assert.AreEqual(resultFr.AvailableStates.Count, 2);

        var availableState1Fr = resultFr.AvailableStates.FirstOrDefault(x => x.Name == stateName1);
        Assert.IsNotNull(availableState1Fr);
        Assert.AreEqual(availableState1Fr.SchemeCode, schemeCode);
        Assert.AreEqual(availableState1Fr.LocalizedName, localizedStateName1Fr);

        var availableState3Fr = resultFr.AvailableStates.FirstOrDefault(x => x.Name == stateName3);
        Assert.IsNotNull(availableState3Fr);
        Assert.AreEqual(availableState3Fr.SchemeCode, schemeCode);
        Assert.AreEqual(availableState3Fr.LocalizedName, localizedStateName3Fr);
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
        
        var request = new GetAvailableStatesToSetBySchemeCodeRequest(schemeCode);
        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, WorkflowApiOperationId.RpcGetAvailableStatesToSetBySchemeCode).WorkflowApiRpcGetAvailableStatesToSetBySchemeCodeAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetAvailableStatesToSetBySchemeCodeRequest(Guid.NewGuid().ToString());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
            await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcGetAvailableStatesToSetBySchemeCodeAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}