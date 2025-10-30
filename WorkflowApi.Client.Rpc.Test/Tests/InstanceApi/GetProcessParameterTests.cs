using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Rpc.Test.Models;
using WorkflowApi.Client.Test.Models;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class GetProcessParameterTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetSystemGuidParameter(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "ProcessId";

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(processId, result.Value);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitGuidParameter(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "GuidParameter";
        var value = Guid.NewGuid();
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert
        
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitGuidParameter(TestService service)
    {
        // Arrange
        
        var name = "GuidParameter";
        var value = Guid.NewGuid();

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(Guid));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitStringParameter(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "StringParameter";
        var value = "Value";

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));
        
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitStringParameter(TestService service)
    {
        // Arrange
        
        var name = "StringParameter";
        var value = "Value";

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(string));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitIntParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);

        var name = "IntParameter";
        var value = 123;

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));
        
        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitIntParameter(TestService service)
    {
        // Arrange
        var name = "IntParameter";
        var value = 123;

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(int));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitDateTimeParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "TimeParameter";
        var value = DateTime.Now;

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitDateTimeParameter(TestService service)
    {
        // Arrange

        var name = "TimeParameter";
        var value = DateTime.Now;

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(DateTime));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitBoolParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "BoolParameter";
        var value = true;

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));
         
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitBoolParameter(TestService service)
    {
        // Arrange

        var name = "BoolParameter";
        var value = true;

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(bool));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitByteArrayParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "ArrayParameter";
        var value = new byte[] {1, 2, 3};

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitByteArrayParameter(TestService service)
    {
        // Arrange

        var name = "ArrayParameter";
        var value = new byte[] {1, 2, 3};

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(byte[]));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));
        
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitStringListParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "ListParameter";
        var value = new List<string> {"One", "Two", "Three"};

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));
        
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitStringListParameter(TestService service)
    {
        // Arrange

        var name = "ListParameter";
        var value = new List<string> {"One", "Two", "Three"};

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(List<string>));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));
        
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitComplexObjectParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "ComplexObjectParameter";

        var value = new ComplexTestObject
        {
            Prop1 = "NestedValue",
            Prop2 = 456,
            Nested = new NestedObject
            {
                SubProp1 = false,
                SubProp2 = [7, 8, 9]
            }
        };

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));
        
        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitComplexObjectParameter(TestService service)
    {
        // Arrange

        var name = "ComplexObjectParameter";

        var value = new ComplexTestObject
        {
            Prop1 = "NestedValue",
            Prop2 = 456,
            Nested = new NestedObject
            {
                SubProp1 = false,
                SubProp2 = [7, 8, 9]
            }
        };

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(ComplexTestObject));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetImplicitDictionaryParameter(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "DictionaryParameter";

        var value = new Dictionary<string, int>
        {
            {"Key1", 1},
            {"Key2", 2},
            {"Key3", 3}
        };

        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldGetExplicitDictionaryParameter(TestService service)
    {
        // Arrange
        
        var name = "DictionaryParameter";
      
        var value = new Dictionary<string, int>
        {
            {"Key1", 1},
            {"Key2", 2},
            {"Key3", 3}
        };

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(Dictionary<string, int>));
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new(value));

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldThrowExceptionIfCannotConvertType(TestService service)
    {
        // Arrange
        
        var name = Guid.NewGuid().ToString();
        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(Guid));
        
        await service.Client.Parameters.WorkflowApiDataProcessesParametersCreateAsync(processId, name, new ParameterCreateRequest("InvalidGuidValue"));

        // Act
        
        var request = new GetProcessParameterRequest(processId, name);
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(request));

        // Assert

        StringAssert.Contains(exception.Message, $"Error converting value");
        Assert.AreEqual(500, exception.ErrorCode);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturnNullValueIfParameterNotFound(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "Test";

        // Act

        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, name));

        // Assert

        AssertParameter(null, result.Value);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);

        // Act
        
        var request = new GetProcessParameterRequest(processId, "ProcessId");
        var result = await service.Client.ExclusivePermissions(c => c.RpcInstance, WorkflowApiOperationId.RpcGetProcessParameter).WorkflowApiRpcGetProcessParameterAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new GetProcessParameterRequest(Guid.NewGuid(), "Test");

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcGetProcessParameterAsync(request));

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    private async Task<Guid> CreateProcessAsync(TestService service)
    {
        var schemeCode = Guid.NewGuid().ToString();
        
        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));
        
        return processId;
    }

    private async Task<Guid> CreateProcessWithExplicitParameterAsync(TestService service, string parameterName, Type parameterType)
    {
        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1")
            .Initial()
            .CreateParameter(parameterName, parameterType, OptimaJet.Workflow.Core.Model.ParameterPurpose.Persistence);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var processId = Guid.NewGuid();
        await service.Client.RpcInstance.WorkflowApiRpcCreateInstanceAsync(new(schemeCode, processId));

        return processId;
    }

    private void AssertParameter(object? expected, object? actual)
    {
        string expectedJson = JsonConvert.SerializeObject(expected, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        });

        var actualJson = Converter.ToJsonString(actual);

        Assert.AreEqual(expectedJson, actualJson);
    }
}