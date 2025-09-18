using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Models;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.SchemeApi;

[TestClass]
public class GetProcessSchemeTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetProcessScheme(TestService service)
    {
        // Arrange
        var processId = Guid.NewGuid();
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder.Create(schemeCode)
          .CreateActivity("Initial").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        // Act

        var response = await service.Client.RpcScheme.WorkflowApiRpcGetProcessSchemeAsync(new (processId));

        // Assert

        Assert.IsNotNull(response);

        var expectedScheme = builder.ProcessDefinition.Serialize();
        var actualScheme = Converter.ToSchemaString(response.ProcessScheme);

        Assert.AreEqual(expectedScheme, actualScheme);
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

        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcScheme, OperationId.RpcGetProcessScheme).WorkflowApiRpcGetProcessSchemeAsync(new(processId));

        // Assert

        Assert.IsNotNull(response);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcIsProcessExistsAsync(new (Guid.NewGuid()))
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
