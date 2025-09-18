using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class IsProcessExistsTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnTrue_WhenInstanceExists(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        
        var processId = Guid.NewGuid();
        
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        
        var request = new IsProcessExistsRequest(processId);
        
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcIsProcessExistsAsync(request);

        // Assert
        
        Assert.IsTrue(result.IsExists);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnFalse_WhenInstanceNotExists(TestService service)
    {
        // Arrange
        
        var request = new IsProcessExistsRequest(Guid.NewGuid());
        
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcIsProcessExistsAsync(request);

        // Assert
        
        Assert.IsFalse(result.IsExists);
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

        var request = new IsProcessExistsRequest(processId);
        
        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcInstance, OperationId.RpcIsProcessExists).WorkflowApiRpcIsProcessExistsAsync(request);

        // Assert
        
        Assert.IsTrue(result.IsExists);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange
        
        var request = new IsProcessExistsRequest(Guid.NewGuid());
        
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcIsProcessExistsAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
