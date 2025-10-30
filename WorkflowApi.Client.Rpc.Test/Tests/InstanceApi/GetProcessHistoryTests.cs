using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class GetProcessHistoryTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnHistory(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial().Ref(out var activity1)
            .CreateActivity("Activity2").Ref(out var activity2)
            .CreateActivity("Activity3").Ref(out var activity3)
            .CreateTransition("Transition1", activity1, activity2)
            .CreateTransition("Transition2", activity2, activity3);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        
        var processId = Guid.NewGuid();
        
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        
        var request = new GetProcessHistoryRequest(processId);
        
        // Act

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessHistoryAsync(request);

        // Assert

        var history = response.History;
        Assert.IsNotNull(history);
        Assert.AreEqual(3, history.Count);

        var firstItem = service.DataProviderId == PersistenceProviderId.Oracle 
            ? history.Single(i => i.FromActivityName == null)
            : history.Single(i => i.FromActivityName == "");
        Assert.AreEqual(processId, firstItem.ProcessId);
        Assert.AreEqual("Initializing", firstItem.TriggerName);
        Assert.AreEqual(service.DataProviderId == PersistenceProviderId.Oracle ? null : "", firstItem.FromActivityName);
        Assert.AreEqual("Activity1", firstItem.ToActivityName);
        Assert.AreEqual(false, firstItem.IsFinalised);
        
        var secondItem = history.Single(i => i.FromActivityName == "Activity1");
        Assert.AreEqual(processId, secondItem.ProcessId);
        Assert.AreEqual(null, secondItem.TriggerName);
        Assert.AreEqual("Activity1", secondItem.FromActivityName);
        Assert.AreEqual("Activity2", secondItem.ToActivityName);
        Assert.AreEqual(false, secondItem.IsFinalised);
        
        var thirdItem = history.Single(i => i.FromActivityName == "Activity2");
        Assert.AreEqual(processId, thirdItem.ProcessId);
        Assert.AreEqual(null, thirdItem.TriggerName);
        Assert.AreEqual("Activity2", thirdItem.FromActivityName);
        Assert.AreEqual("Activity3", thirdItem.ToActivityName);
        Assert.AreEqual(false, thirdItem.IsFinalised);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnHistory_WithPaging(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial().Ref(out var activity1)
            .CreateActivity("Activity2").Ref(out var activity2)
            .CreateActivity("Activity3").Ref(out var activity3)
            .CreateTransition("Transition1", activity1, activity2)
            .CreateTransition("Transition2", activity2, activity3);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        
        var processId = Guid.NewGuid();
        
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        
        var request = new GetProcessHistoryRequest(processId, new Paging(0, 2));
        
        // Act

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessHistoryAsync(request);

        // Assert

        var history = response.History;
        Assert.IsNotNull(history);
        Assert.AreEqual(2, history.Count);

        var firstItem = service.DataProviderId == PersistenceProviderId.Oracle 
            ? history.FirstOrDefault(i => i.FromActivityName == null)
            : history.FirstOrDefault(i => i.FromActivityName == "");
        
        if (firstItem != null)
        {
            Assert.AreEqual(processId, firstItem.ProcessId);
            Assert.AreEqual("Initializing", firstItem.TriggerName);
            Assert.AreEqual(service.DataProviderId == PersistenceProviderId.Oracle ? null : "", firstItem.FromActivityName);
            Assert.AreEqual("Activity1", firstItem.ToActivityName);
            Assert.AreEqual(false, firstItem.IsFinalised);
        }
        
        
        
        var secondItem = history.FirstOrDefault(i => i.FromActivityName == "Activity1");
        
        if (secondItem != null)
        {
            Assert.AreEqual(processId, secondItem.ProcessId);
            Assert.AreEqual(null, secondItem.TriggerName);
            Assert.AreEqual("Activity1", secondItem.FromActivityName);
            Assert.AreEqual("Activity2", secondItem.ToActivityName);
            Assert.AreEqual(false, secondItem.IsFinalised);
        }
        
        var thirdItem = history.FirstOrDefault(i => i.FromActivityName == "Activity2");
        
        if (thirdItem != null)
        {
            Assert.AreEqual(processId, thirdItem.ProcessId);
            Assert.AreEqual(null, thirdItem.TriggerName);
            Assert.AreEqual("Activity2", thirdItem.FromActivityName);
            Assert.AreEqual("Activity3", thirdItem.ToActivityName);
            Assert.AreEqual(false, thirdItem.IsFinalised);
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
            .CreateActivity("Activity1").Initial().Ref(out var activity1)
            .CreateActivity("Activity2").Ref(out var activity2)
            .CreateActivity("Activity3").Ref(out var activity3)
            .CreateTransition("Transition1", activity1, activity2)
            .CreateTransition("Transition2", activity2, activity3);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        
        var processId = Guid.NewGuid();
        
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        
        var request = new GetProcessHistoryRequest(processId);
        
        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcInstance, WorkflowApiOperationId.RpcGetProcessHistory).WorkflowApiRpcGetProcessHistoryAsync(request);

        // Assert

        var history = response.History;
        Assert.IsNotNull(history);
        Assert.AreEqual(3, history.Count);

        var firstItem = service.DataProviderId == PersistenceProviderId.Oracle 
            ? history.Single(i => i.FromActivityName == null)
            : history.Single(i => i.FromActivityName == "");
        Assert.AreEqual(processId, firstItem.ProcessId);
        Assert.AreEqual("Initializing", firstItem.TriggerName);
        Assert.AreEqual(service.DataProviderId == PersistenceProviderId.Oracle ? null : "", firstItem.FromActivityName);
        Assert.AreEqual("Activity1", firstItem.ToActivityName);
        Assert.AreEqual(false, firstItem.IsFinalised);
        
        var secondItem = history.Single(i => i.FromActivityName == "Activity1");
        Assert.AreEqual(processId, secondItem.ProcessId);
        Assert.AreEqual(null, secondItem.TriggerName);
        Assert.AreEqual("Activity1", secondItem.FromActivityName);
        Assert.AreEqual("Activity2", secondItem.ToActivityName);
        Assert.AreEqual(false, secondItem.IsFinalised);
        
        var thirdItem = history.Single(i => i.FromActivityName == "Activity2");
        Assert.AreEqual(processId, thirdItem.ProcessId);
        Assert.AreEqual(null, thirdItem.TriggerName);
        Assert.AreEqual("Activity2", thirdItem.FromActivityName);
        Assert.AreEqual("Activity3", thirdItem.ToActivityName);
        Assert.AreEqual(false, thirdItem.IsFinalised);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange
        
        var request = new GetProcessHistoryRequest(Guid.NewGuid());
        
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcGetProcessHistoryAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
