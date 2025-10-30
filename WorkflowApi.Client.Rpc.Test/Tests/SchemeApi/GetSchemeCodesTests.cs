using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.SchemeApi;

[TestClass]
public class GetSchemeCodesTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldRpcGetSchemeCodes(TestService service)
    {
        // Arrange

        List<string> schemeCodes = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()];

        foreach (var schemeCode in schemeCodes)
        {
            var builder = ProcessDefinitionBuilder.Create(schemeCode)
                .CreateActivity("Initial").Initial();

            await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        }

        // Act

        var response = await service.Client.RpcScheme.WorkflowApiRpcGetSchemeCodesAsync(new());

        // Assert

        Assert.IsNotNull(response);

        foreach (var schemeCode in schemeCodes)
        {
            Assert.IsTrue(response.SchemeCodes.Contains(schemeCode));
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldRpcGetSchemeCodes_WithTags(TestService service)
    {
        // Arrange

        var tag = Guid.NewGuid().ToString();
        List<string> schemeCodes = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()];

        foreach (var schemeCode in schemeCodes)
        {
            var builder = ProcessDefinitionBuilder.Create(schemeCode)
                .CreateActivity("Initial").Initial();

            await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder, [tag]);
        }

        // Act

        var response = await service.Client.RpcScheme.WorkflowApiRpcGetSchemeCodesAsync(new([tag]));

        // Assert

        Assert.IsNotNull(response);
        CollectionAssert.AreEquivalent(schemeCodes, response.SchemeCodes);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcScheme, WorkflowApiOperationId.RpcGetSchemeCodes).WorkflowApiRpcGetSchemeCodesAsync(new());

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcScheme, Array.Empty<string>()).WorkflowApiRpcGetSchemeCodesAsync(new())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
