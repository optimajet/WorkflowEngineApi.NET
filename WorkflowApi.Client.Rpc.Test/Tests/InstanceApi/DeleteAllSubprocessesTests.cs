using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class DeleteAllSubprocessesTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldDeleteAllSubprocesses(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var rootProcessId = Guid.NewGuid();
        await CreateInitialData(service, schemeCode, rootProcessId);

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceTreeAsync(new (rootProcessId));
        var processIds = GetAllChildIds(response.ProcessInstanceTree).Where(x => x != rootProcessId).ToList();

        foreach (var processId in processIds)
        {
            var processInstance = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new (processId));
            Assert.IsNotNull(processInstance.ProcessInstance);
        }

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcDeleteAllSubprocessesAsync(new (rootProcessId));

        // Assert

        Assert.AreEqual(2, processIds.Count);
        await AssertDeletedProcesses(service, processIds);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();
        var rootProcessId = Guid.NewGuid();
        await CreateInitialData(service, schemeCode, rootProcessId);

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceTreeAsync(new (rootProcessId));
        var processIds = GetAllChildIds(response.ProcessInstanceTree).Where(x => x != rootProcessId).ToList();

        // Act

        await service.Client.ExclusivePermissions(c => c.RpcInstance, OperationId.RpcDeleteAllSubprocesses).WorkflowApiRpcDeleteAllSubprocessesAsync(new (rootProcessId));

        // Assert

        Assert.AreEqual(2, processIds.Count);
        await AssertDeletedProcesses(service, processIds);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new DeleteAllSubprocessesRequest(Guid.NewGuid());

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcDeleteAllSubprocessesAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    private static async Task CreateInitialData(TestService service, string schemeCode, Guid rootProcessId)
    {
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

    private static async Task AssertDeletedProcesses(TestService service, List<Guid> processIds)
    {
        foreach (var processId in processIds)
        {
            var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new (processId)));
            Assert.AreEqual("ProcessNotFoundException", GetExceptionType(exception));
        }
    }

    private static string GetExceptionType(ApiException exception)
    {
        var errorContent = JObject.Parse(exception.ErrorContent.ToString()!);
        var exceptionType = errorContent["exception"]!["type"]!.ToString();
        return exceptionType;
    }

    public static List<Guid> GetAllChildIds(ProcessInstancesTree processInstancesTree)
    {
        var ids = new List<Guid> {processInstancesTree.Id};
        foreach (var child in processInstancesTree.Children)
        {
            ids.AddRange(GetAllChildIds(child));
        }
        return ids;
    }
}