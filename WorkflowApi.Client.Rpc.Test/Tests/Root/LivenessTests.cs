using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.Root;

[TestClass]
public class LivenessTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        await service.Client.RootApi.WorkflowApiLivenessAsync();
        //Exception will be thrown on unsuccessful response
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        await service.Client.ExclusivePermissions(c => c.RootApi, [WorkflowApiOperationId.Liveness]).WorkflowApiLivenessAsync();
        
        //Exception will be thrown on unsuccessful response
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RootApi, Array.Empty<string>()).WorkflowApiLivenessAsync()
        );
        
        Assert.AreEqual(403, exception.ErrorCode);
    }
}