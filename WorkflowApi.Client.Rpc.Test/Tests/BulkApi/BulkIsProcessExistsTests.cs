using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.BulkApi;

[TestClass]
public class BulkIsProcessExistsTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldCheckProcessesExistence(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var request = new BulkIsProcessExistsRequest([Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]);

        foreach (var processId in request.ProcessIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));
        }

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkIsProcessExistsAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsTrue(result.Result);
            Assert.IsNull(result.Exception);
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

        var request = new BulkIsProcessExistsRequest([processId]);

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcBulk, OperationId.RpcBulkIsProcessExists).WorkflowApiRpcBulkIsProcessExistsAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new BulkIsProcessExistsRequest([]);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcBulk, Array.Empty<string>()).WorkflowApiRpcBulkIsProcessExistsAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
