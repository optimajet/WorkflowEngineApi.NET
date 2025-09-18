using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.SchemeApi;

[TestClass]
public class UpdateSchemeIfObsoleteTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldUpdateSchemeIfObsolete(TestService service)
    {
        // Arrange
        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        
        await service.Client.RpcScheme.WorkflowApiRpcSetSchemeIsObsoleteAsync(new(schemeCode));

        var scheme1 = await service.Client.RpcScheme.WorkflowApiRpcGetProcessSchemeAsync(new(processId));
        Assert.IsTrue(scheme1.ProcessScheme.IsObsolete);
        
        // Act

        await service.Client.RpcScheme.WorkflowApiRpcUpdateSchemeIfObsoleteAsync(new (processId, true));

        // Assert

        var scheme2 = await service.Client.RpcScheme.WorkflowApiRpcGetProcessSchemeAsync(new(processId));
        Assert.IsFalse(scheme2.ProcessScheme.IsObsolete);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange
        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
            .CreateActivity("Initial").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        await service.Client.RpcScheme.WorkflowApiRpcSetSchemeIsObsoleteAsync(new(schemeCode));

        var scheme1 = await service.Client.RpcScheme.WorkflowApiRpcGetProcessSchemeAsync(new(processId));
        Assert.IsTrue(scheme1.ProcessScheme.IsObsolete);

        // Act

        await service.Client.ExclusivePermissions(c => c.RpcScheme, OperationId.RpcUpdateSchemeIfObsolete).WorkflowApiRpcUpdateSchemeIfObsoleteAsync(new(processId));

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
            async () => await service.Client.ExclusivePermissions(c => c.RpcScheme, Array.Empty<string>()).WorkflowApiRpcUpdateSchemeIfObsoleteAsync(new (Guid.NewGuid()))
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
