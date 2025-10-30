using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class CheckAllSubprocessesCompletedTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnTrueIfSubprocessesNotExist(TestService service)
    {
        // Arrange

        var rootProcessId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();
        var rootBuilder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity").Initial();
        await service.Client.Schemes.CreateSchemeFromBuilderAsync(rootBuilder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, rootProcessId));
        
        var processInstance = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new (rootProcessId));
        Assert.IsNotNull(processInstance.ProcessInstance);

        // Act

        var checkResponse = await service.Client.RpcInstance.WorkflowApiRpcCheckAllSubprocessesCompletedAsync(new (rootProcessId));

        // Assert

        Assert.IsTrue(checkResponse.IsAllSubprocessesCompleted);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnFalseIfSubprocessesExist(TestService service)
    {
        // Arrange

        var rootProcessId = Guid.NewGuid();
        await CreateInitialWithSubprocessesData(service, rootProcessId);

        var processInstance = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new (rootProcessId));
        Assert.IsNotNull(processInstance.ProcessInstance);

        // Act

        var checkResponse = await service.Client.RpcInstance.WorkflowApiRpcCheckAllSubprocessesCompletedAsync(new (rootProcessId));

        // Assert

        Assert.IsFalse(checkResponse.IsAllSubprocessesCompleted);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var rootProcessId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();
        var rootBuilder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity").Initial();
        await service.Client.Schemes.CreateSchemeFromBuilderAsync(rootBuilder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, rootProcessId));
        
        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcInstance, WorkflowApiOperationId.RpcCheckAllSubprocessesCompleted)
            .WorkflowApiRpcCheckAllSubprocessesCompletedAsync(new (rootProcessId));

        // Assert

        Assert.IsTrue(response.IsAllSubprocessesCompleted);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcCheckAllSubprocessesCompletedAsync(new (Guid.NewGuid())));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    private static async Task CreateInitialWithSubprocessesData(TestService service, Guid rootProcessId)
    {
        var schemeCode = Guid.NewGuid().ToString();
        var transition1Name = "Transition1";
        var transition2Name = "Transition2";
        var rootBuilder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial().Ref(out var activity1)
            .CreateActivity("Activity2").Ref(out var activity2)
            .CreateActivity("Activity3").Ref(out var activity3)
            .CreateTransition(transition1Name, activity1, activity2)
            .ParallelStart()
            .CreateTransition(transition2Name, activity1, activity3)
            .ParallelStart();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(rootBuilder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, rootProcessId));
    }
}