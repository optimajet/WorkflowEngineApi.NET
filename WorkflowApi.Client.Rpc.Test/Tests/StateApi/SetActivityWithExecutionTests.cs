using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using ParameterPurpose = OptimaJet.Workflow.Core.Model.ParameterPurpose;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class SetActivityWithExecutionTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetActivityWithExecution(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var processId = Guid.NewGuid();
        var identityId = Guid.NewGuid().ToString();
        var impersonatedIdentityId = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Activity2").Ref(out var activity2)
            .CreateActivity("Activity3").Final().Ref(out var activity3)
            .CreateTransition("Transition1", activity2, activity3)
            .Direct()
            .CreateParameter("Fourth", typeof(Guid), ParameterPurpose.Persistence);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        List<ProcessParameter> testProcessParameters =
        [
            new("First", "Value", ParameterPurposeWithoutSystem.Persistence),
            new("Second", 12, ParameterPurposeWithoutSystem.Persistence),
            new("Third", Guid.NewGuid(), ParameterPurposeWithoutSystem.Persistence),
            new("Fourth", Guid.NewGuid(), ParameterPurposeWithoutSystem.Temporary),
            new("Fifth", Guid.NewGuid(), ParameterPurposeWithoutSystem.Temporary)
        ];

        // Act

        var request = new SetActivityWithExecutionRequest(processId, "Activity2", testProcessParameters, identityId, impersonatedIdentityId, true);
        await service.Client.RpcStates.WorkflowApiRpcSetActivityWithExecutionAsync(request);

        // Assert

        var processStatus = await service.Client.RpcInstance.WorkflowApiRpcGetProcessStatusAsync(new(processId));
        Assert.IsNotNull(processStatus);
        Assert.AreEqual(ProcessStatus.Finalized, processStatus.ProcessStatus);

        var processInstance = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new(request.ProcessId));
        Assert.IsNotNull(processInstance);
        Assert.AreEqual("Activity3", processInstance.ProcessInstance.CurrentActivityName);

        var expectedFirstParameter = testProcessParameters.Single(p => p.Name == "First");
        var firstParameter = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(request.ProcessId, expectedFirstParameter.Name);
        Assert.IsNotNull(firstParameter);
        AssertParameter(expectedFirstParameter.Value, firstParameter.Value);

        var expectedSecondParameter = testProcessParameters.Single(p => p.Name == "Second");
        var secondParameter = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(request.ProcessId, expectedSecondParameter.Name);
        Assert.IsNotNull(secondParameter);
        AssertParameter(expectedSecondParameter.Value, secondParameter.Value);

        var expectedThirdParameter = testProcessParameters.Single(p => p.Name == "Third");
        var thirdParameter = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(request.ProcessId, expectedThirdParameter.Name);
        Assert.IsNotNull(thirdParameter);
        AssertParameter(expectedThirdParameter.Value, thirdParameter.Value);

        var expectedFourthParameter = testProcessParameters.Single(p => p.Name == "Fourth");
        var fourthParameter = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(request.ProcessId, expectedFourthParameter.Name);
        Assert.IsNotNull(fourthParameter);
        AssertParameter(expectedFourthParameter.Value, fourthParameter.Value);
        
        var expectedFifthParameter = testProcessParameters.Single(p => p.Name == "Fifth");
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(request.ProcessId, expectedFifthParameter.Name));
        Assert.AreEqual(exception.ErrorCode, 404); // Temporary parameter should be empty
        StringAssert.Contains(exception.Message, $"Parameter with process id {processId} and name {expectedFifthParameter.Name} not found");

        //TODO check IdentityId, ImpersonatedIdentityId
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
            .Initial()
            .CreateActivity("Activity2");

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();

        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        var request = new SetActivityWithExecutionRequest(processId, "Activity2", []);

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, OperationId.RpcSetActivityWithExecution).WorkflowApiRpcSetActivityWithExecutionAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new SetActivityWithExecutionRequest();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcSetActivityWithExecutionAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    private void AssertParameter(object? expected, object? actual)
    {
        string expectedJson = JsonConvert.SerializeObject(expected);
        string actualJson = JsonConvert.SerializeObject(actual);

        expectedJson = expectedJson.ToLowerInvariant();
        actualJson = actualJson.ToLowerInvariant();

        Assert.AreEqual(expectedJson, actualJson);
    }
}