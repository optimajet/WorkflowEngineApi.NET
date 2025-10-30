using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class SetActivityWithoutExecutionTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetActivityWithoutExecutionTests(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity("Activity2").Ref(out var activity2)
            .CreateActivity("Activity3").Ref(out var activity3)
            .CreateTransition("Transition1", activity2, activity3)
            .Direct();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new SetActivityWithoutExecutionRequest(processId, "Activity2");
        var result = await service.Client.RpcStates.WorkflowApiRpcSetActivityWithoutExecutionAsync(request);

        // Assert

        Assert.IsNotNull(result);
        var data = await service.Client.Processes.WorkflowApiDataProcessesGetAsync(request.ProcessId);
        Assert.IsNotNull(data);
        Assert.AreEqual("Activity2", data.ActivityName);
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

        // Act
        
        var request = new SetActivityWithoutExecutionRequest(processId, "Activity2");
        var result = await service.Client.ExclusivePermissions(c => c.RpcStates, WorkflowApiOperationId.RpcSetActivityWithoutExecution).WorkflowApiRpcSetActivityWithoutExecutionAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new SetActivityWithoutExecutionRequest();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>()).WorkflowApiRpcSetActivityWithoutExecutionAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}