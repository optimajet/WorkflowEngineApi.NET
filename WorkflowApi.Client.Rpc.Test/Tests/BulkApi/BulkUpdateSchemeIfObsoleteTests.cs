using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.BulkApi;

[TestClass]
public class BulkUpdateSchemeIfObsoleteTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldUpdateSchemeIfObsolete(TestService service)
    {
        // Arrange
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcScheme.WorkflowApiRpcSetSchemeIsObsoleteAsync(new(schemeCode));

        var request = new BulkUpdateSchemeIfObsoleteRequest([Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()], true);

        foreach (var processId in request.ProcessIds)
        {
            await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new (schemeCode, processId));

            await service.Client.RpcScheme.WorkflowApiRpcSetSchemeIsObsoleteAsync(new(schemeCode));

            var scheme = await service.Client.RpcScheme.WorkflowApiRpcGetProcessSchemeAsync(new(processId));
            Assert.IsTrue(scheme.ProcessScheme.IsObsolete);
        }
        
        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkUpdateSchemeIfObsoleteAsync(request);

        // Assert

        foreach (var processId in request.ProcessIds)
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNotNull(result.Result);
            Assert.IsNull(result.Exception);

            var scheme = await service.Client.RpcScheme.WorkflowApiRpcGetProcessSchemeAsync(new(processId));
            Assert.IsFalse(scheme.ProcessScheme.IsObsolete);
        }
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange


        // Act

        await service.Client.ExclusivePermissions(c => c.RpcBulk, OperationId.RpcBulkUpdateSchemeIfObsolete).WorkflowApiRpcBulkUpdateSchemeIfObsoleteAsync(new([]));

        // Assert

        // Exception is not thrown, so the test passes
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange


        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcBulk, Array.Empty<string>()).WorkflowApiRpcBulkUpdateSchemeIfObsoleteAsync(new ([]))
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
