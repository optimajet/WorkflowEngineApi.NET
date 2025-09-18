using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class DeleteInstanceTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldDeleteExistedInstance(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        
        var processId = Guid.NewGuid();
        
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        
        var exists = await service.Client.RpcInstance.WorkflowApiRpcIsProcessExistsAsync(new (processId));
        
        Assert.IsTrue(exists.IsExists);

        var request = new DeleteInstanceRequest(processId);
        
        // Act

        await service.Client.RpcInstance.WorkflowApiRpcDeleteInstanceAsync(request);

        // Assert
        
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.Processes.WorkflowApiDataProcessesGetAsync(request.ProcessId)
        );

        Assert.AreEqual(404, exception.ErrorCode);
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
        
        var exists = await service.Client.RpcInstance.WorkflowApiRpcIsProcessExistsAsync(new (processId));
        
        Assert.IsTrue(exists.IsExists);
        
        var request = new DeleteInstanceRequest(processId);
        
        // Act

        await service.Client.ExclusivePermissions(c => c.RpcInstance, OperationId.RpcDeleteInstance).WorkflowApiRpcDeleteInstanceAsync(request);

        // Assert
        
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.Processes.WorkflowApiDataProcessesGetAsync(request.ProcessId)
        );

        Assert.AreEqual(404, exception.ErrorCode);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange
        
        var request = new DeleteInstanceRequest(Guid.NewGuid());
        
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcDeleteInstanceAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
