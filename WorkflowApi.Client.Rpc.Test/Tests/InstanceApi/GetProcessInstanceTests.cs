using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class GetProcessInstanceTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnProcessInstance(TestService service)
    {
        // Arrange
        
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);
        
        var createRequest = new CreateInstanceRequest
        {
            ProcessId = Guid.NewGuid(),
            SchemeCode = schemeCode,
            IdentityId = Guid.NewGuid().ToString(),
            ImpersonatedIdentityId = Guid.NewGuid().ToString(),
            CalendarName = Guid.NewGuid().ToString(),
            InitialProcessParameters =
            [
                new("First", "Value", ParameterPurposeWithoutSystem.Persistence),
                new("Second", 123, ParameterPurposeWithoutSystem.Persistence),
                new("Third", DateTime.Now, ParameterPurposeWithoutSystem.Persistence),
                new("Fourth", true, ParameterPurposeWithoutSystem.Persistence),
                new("Fifth", new byte[] {1, 2, 3}, ParameterPurposeWithoutSystem.Persistence),
                new("Sixth", new List<string> {"One", "Two", "Three"}, ParameterPurposeWithoutSystem.Persistence)
            ]
        };
        
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(createRequest);

        var request = new GetProcessInstanceRequest(createRequest.ProcessId);
        
        // Act

        var response = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(request);

        // Assert
        
        var processInstance = response.ProcessInstance;
        Assert.IsNotNull(processInstance);
        Assert.AreEqual(createRequest.ProcessId, processInstance.ProcessId);
        Assert.AreEqual(createRequest.SchemeCode, processInstance.SchemeCode);
        Assert.AreEqual(createRequest.CalendarName, processInstance.CalendarName);
        
        foreach (var processParameter in createRequest.InitialProcessParameters)
        {
            var parameter = processInstance.ProcessParameters.FirstOrDefault(p => p.Name == processParameter.Name);
            Assert.IsNotNull(parameter);
            Assert.AreEqual(processParameter.Name, parameter.Name);
            Assert.AreEqual(JsonConvert.SerializeObject(processParameter.Value), parameter.Value as string);
            Assert.AreEqual(processParameter.Purpose.ToString(), parameter.Purpose.ToString());
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
        
        var request = new GetProcessInstanceRequest
        {
            ProcessId = processId
        };
        
        // Act

        var response = await service.Client.ExclusivePermissions(c => c.RpcInstance, OperationId.RpcGetProcessInstance).WorkflowApiRpcGetProcessInstanceAsync(request);

        // Assert
        
        var processInstance = response.ProcessInstance;
        Assert.IsNotNull(processInstance);
        Assert.AreEqual(request.ProcessId, processInstance.ProcessId);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange
        
        var request = new GetProcessInstanceRequest(Guid.NewGuid());
        
        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcGetProcessInstanceAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}
