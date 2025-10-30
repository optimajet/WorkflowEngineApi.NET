using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using BulkTaskState = WorkflowApi.Client.Model.BulkTaskState;

namespace WorkflowApi.Client.Rpc.Test.Tests.BulkApi;

[TestClass]
public class BulkDeleteInstanceTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldDeleteInstances(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var request = new BulkDeleteInstanceRequest([Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]);

        foreach (var processId in request.ProcessIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));

            var exists = await service.Client.RpcInstance.WorkflowApiRpcIsProcessExistsAsync(new (processId));

            Assert.IsTrue(exists.IsExists);
        }

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkDeleteInstanceAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNotNull(result.Result);
            Assert.IsNull(result.Exception);

            var exists = await service.Client.RpcInstance.WorkflowApiRpcIsProcessExistsAsync(new (processId));

            Assert.IsFalse(exists.IsExists);
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

        var exists = await service.Client.RpcInstance.WorkflowApiRpcIsProcessExistsAsync(new (processId));

        Assert.IsTrue(exists.IsExists);

        var request = new BulkDeleteInstanceRequest([processId]);

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcBulk, WorkflowApiOperationId.RpcBulkDeleteInstance).WorkflowApiRpcBulkDeleteInstanceAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new BulkDeleteInstanceRequest([]);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcBulk, Array.Empty<string>()).WorkflowApiRpcBulkDeleteInstanceAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
