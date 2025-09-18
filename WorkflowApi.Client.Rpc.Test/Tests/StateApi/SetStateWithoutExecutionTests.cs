using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.StateApi;

[TestClass]
public class SetStateWithoutExecutionTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetStateWithoutExecution(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var firstState = "Activity1";
        var secondState = "Activity2";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Initial")
            .Initial()
            .CreateActivity(firstState)
            .State(firstState)
            .EnableSetState()
            .CreateActivity(secondState)
            .State(secondState)
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new SetStateWithoutExecutionRequest(processId, secondState);
        await service.Client.RpcStates.WorkflowApiRpcSetStateWithoutExecutionAsync(request);

        // Assert

        var processInstance = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new(processId));
        Assert.AreEqual(secondState, processInstance.ProcessInstance.CurrentState);

        var parameter3 = processInstance.ProcessInstance.ProcessParameters.FirstOrDefault(p => p.Name == "TestParam3");
        Assert.IsNull(parameter3);
        var parameter4 = processInstance.ProcessInstance.ProcessParameters.FirstOrDefault(p => p.Name == "TestParam4");
        Assert.IsNull(parameter4);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var activityName = "Activity2";

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateActivity(activityName)
            .State(activityName)
            .EnableSetState();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var request = new SetStateWithoutExecutionRequest(processId, activityName);
        await service.Client.ExclusivePermissions(c => c.RpcStates, OperationId.RpcSetStateWithoutExecution)
            .WorkflowApiRpcSetStateWithoutExecutionAsync(request);

        // Assert

        var processInstance = await service.Client.Processes.WorkflowApiDataProcessesGetAsync(processId);
        Assert.AreEqual(activityName, processInstance.StateName);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new SetStateWithoutExecutionRequest(Guid.NewGuid(), "123");

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
            await service.Client.ExclusivePermissions(c => c.RpcStates, Array.Empty<string>())
                .WorkflowApiRpcSetStateWithoutExecutionAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}