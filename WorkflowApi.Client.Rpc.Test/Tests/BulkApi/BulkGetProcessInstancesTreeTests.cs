using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using BulkTaskState = WorkflowApi.Client.Model.BulkTaskState;

namespace WorkflowApi.Client.Rpc.Test.Tests.BulkApi;

[TestClass]
public class BulkGetProcessInstancesTreeTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetInstancesTrees(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var transition1Name = "Transition1";
        var transition2Name = "Transition2";
        var transition3Name = "Transition3";

        var rootBuilder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial().Ref(out var activity1)
            .CreateActivity("Activity2").Ref(out var activity2)
            .CreateActivity("Activity3").Ref(out var activity3)
            .CreateActivity("Activity4").Ref(out var activity4)
            .CreateTransition(transition1Name, activity1, activity2)
            .ParallelStart()
            .CreateTransition(transition2Name, activity1, activity3)
            .ParallelStart()
            .CreateTransition(transition3Name, activity3, activity4)
            .ParallelStart();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(rootBuilder);

        var request = new BulkGetProcessInstancesTreeRequest([Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]);

        foreach (var processId in request.RootProcessIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        }

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkGetProcessInstancesTreeAsync(request);

        // Assert

        foreach (var processId in request.RootProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNull(result.Exception);

            var instanceTree = result.Result;

            Assert.IsTrue(instanceTree.IsRoot);
            Assert.IsNull(instanceTree.StartingTransitionName);
            Assert.AreEqual(processId, instanceTree.Id);
            Assert.AreEqual(2, instanceTree.Children.Count);

            var subprocess1 = instanceTree.Children.Single(c => c.Name == transition1Name);
            Assert.IsFalse(subprocess1.IsRoot);
            Assert.AreEqual(transition1Name, subprocess1.StartingTransitionName);
            Assert.AreEqual(0, subprocess1.Children.Count);

            var subprocess2 = instanceTree.Children.Single(c => c.Name == transition2Name);
            Assert.IsFalse(subprocess2.IsRoot);
            Assert.AreEqual(transition2Name, subprocess2.StartingTransitionName);
            Assert.AreEqual(1, subprocess2.Children.Count);

            var subprocess3 = subprocess2.Children.Single(c => c.Name == transition3Name);
            Assert.IsFalse(subprocess3.IsRoot);
            Assert.AreEqual(transition3Name, subprocess3.StartingTransitionName);
            Assert.AreEqual(0, subprocess3.Children.Count);
        }
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

        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));

        var request = new BulkGetProcessInstancesTreeRequest([processId]);

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcBulk, WorkflowApiOperationId.RpcBulkGetProcessInstancesTree).WorkflowApiRpcBulkGetProcessInstancesTreeAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new BulkGetProcessInstancesTreeRequest([]);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcBulk, Array.Empty<string>()).WorkflowApiRpcBulkGetProcessInstancesTreeAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
