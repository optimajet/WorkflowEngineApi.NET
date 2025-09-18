using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class GetProcessInstanceTreeTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnProcessInstancesTree(TestService service)
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
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));

        // Act

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceTreeAsync(new (processId));

        // Assert

        var instanceTree = response.ProcessInstanceTree;
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

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnProcessInstancesTree_ByChildProcessId(TestService service)
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
        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));

        var rootResponse = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceTreeAsync(new (processId));

        var childProcessId = rootResponse.ProcessInstanceTree.Children.First().Id;

        // Act

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceTreeAsync(new (childProcessId));

        // Assert

        var instanceTree = response.ProcessInstanceTree;
        Assert.IsTrue(instanceTree.IsRoot);
        Assert.IsNull(instanceTree.StartingTransitionName);
        Assert.AreEqual(instanceTree.Id, processId);
        Assert.AreEqual(instanceTree.Children.Count, 2);

        var subprocess1 = instanceTree.Children.Single(c => c.Name == transition1Name);
        Assert.IsFalse(subprocess1.IsRoot);
        Assert.AreEqual(subprocess1.StartingTransitionName, transition1Name);
        Assert.AreEqual(subprocess1.Children.Count, 0);

        var subprocess2 = instanceTree.Children.Single(c => c.Name == transition2Name);
        Assert.IsFalse(subprocess2.IsRoot);
        Assert.AreEqual(subprocess2.StartingTransitionName, transition2Name);
        Assert.AreEqual(subprocess2.Children.Count, 1);

        var subprocess3 = subprocess2.Children.Single(c => c.Name == transition3Name);
        Assert.IsFalse(subprocess3.IsRoot);
        Assert.AreEqual(subprocess3.StartingTransitionName, transition3Name);
        Assert.AreEqual(subprocess3.Children.Count, 0);
        Console.WriteLine(1);
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
        var request = new GetProcessInstancesTreeRequest
        {
            ProcessId = processId
        };

        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcInstance, OperationId.RpcGetProcessInstanceTree)
            .WorkflowApiRpcGetProcessInstanceTreeAsync(request);

        // Assert

        var processInstanceTree = response.ProcessInstanceTree;
        Assert.IsNotNull(processInstanceTree);
        Assert.AreEqual(request.ProcessId, processInstanceTree.Id);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetProcessInstancesTreeRequest
        {
            ProcessId = Guid.NewGuid()
        };
        
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcGetProcessInstanceTreeAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}