using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Root;

[TestClass]
public class LivenessTests
{
    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        await service.Client.RootApi.WorkflowApiLivenessAsync();
    }

    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        await service.Client
            .WithPermissions(c => c.RootApi, [WorkflowApiOperationId.Liveness], [service.TenantId])
            .WorkflowApiLivenessAsync();
    }
    
    [ClientTest(HostId.MultiTenantHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client
                .WithPermissions(c => c.RootApi, Array.Empty<string>(), [service.TenantId])
                .WorkflowApiLivenessAsync()
        );
        
        Assert.AreEqual(403, exception.ErrorCode);
    }
}
