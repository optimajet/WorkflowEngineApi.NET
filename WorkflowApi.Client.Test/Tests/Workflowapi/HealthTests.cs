using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.WorkflowApi;

[TestClass]
public class HealthTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        await service.Client.WorkflowApi.WorkflowApiHealthAsync();
        //Exception will be thrown on unsuccessful response
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        var token = await service.Client.CreateTokenAsync(["workflow-api.health"]);
        var api = service.Client.WorkflowApi;
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";

        await api.WorkflowApiHealthAsync();
        
        //Exception will be thrown on unsuccessful response
    }
    
    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        var token = await service.Client.CreateTokenAsync([]);
        var api = service.Client.WorkflowApi;
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiHealthAsync()
        );
        
        Assert.AreEqual(403, exception.ErrorCode);
    }
}