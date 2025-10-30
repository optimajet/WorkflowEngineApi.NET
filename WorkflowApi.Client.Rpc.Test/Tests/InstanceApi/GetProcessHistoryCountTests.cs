using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class GetProcessHistoryCountTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnHistoryCount(TestService service)
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
        
        var request = new GetProcessHistoryCountRequest(processId);
        
        // Act

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessHistoryCountAsync(request);

        // Assert

        var count = response.ProcessHistoryCount;
        Assert.AreEqual(3, count);
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
        
        var request = new GetProcessHistoryCountRequest(processId);
        
        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcInstance, WorkflowApiOperationId.RpcGetProcessHistoryCount).WorkflowApiRpcGetProcessHistoryCountAsync(request);

        // Assert

        var count = response.ProcessHistoryCount;
        Assert.AreEqual(3, count);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange
        
        var request = new GetProcessHistoryCountRequest(Guid.NewGuid());
        
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcGetProcessHistoryCountAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
