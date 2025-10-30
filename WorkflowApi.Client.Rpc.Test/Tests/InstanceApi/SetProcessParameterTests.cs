using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Rpc.Test.Models;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Rpc.Test.Tests.InstanceApi;

[TestClass]
public class SetProcessParameterTests
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitGuidParameter(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "GuidParameter";
        var value = Guid.NewGuid();

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitGuidParameter(TestService service)
    {
        // Arrange
        var name = "GuidParameter";
        var value = Guid.NewGuid();

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(Guid));

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));
        
        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitStringParameter(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "StringParameter";
        var value = "Value";

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);

        AssertParameter(value, result.Value);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitStringParameter(TestService service)
    {
        // Arrange

        var name = "StringParameter";
        var value = "Value";

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(string));

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);

        AssertParameter(value, result.Value);
    }


    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitIntParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);

        var name = "IntParameter";
        var value = 123;

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitIntParameter(TestService service)
    {
        // Arrange
        var name = "IntParameter";
        var value = 123;

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(int));

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitDateTimeParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "TimeParameter";
        var value = DateTime.Now;

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitDateTimeParameter(TestService service)
    {
        // Arrange

        var name = "TimeParameter";
        var value = DateTime.Now;

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(DateTime));

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitBoolParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "BoolParameter";
        var value = true;

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitBoolParameter(TestService service)
    {
        // Arrange

        var name = "BoolParameter";
        var value = true;

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(bool));

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitByteArrayParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "ArrayParameter";
        var value = new byte[] {1, 2, 3};
        
        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitByteArrayParameter(TestService service)
    {
        // Arrange

        var name = "ArrayParameter";
        var value = new byte[] {1, 2, 3};

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(byte[]));

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));
        
        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitStringListParameter(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var name = "ListParameter";
        var value = new List<string> {"One", "Two", "Three"};

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitStringListParameter(TestService service)
    {
        // Arrange

        var name = "ListParameter";
        var value = new List<string> {"One", "Two", "Three"};

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(List<string>));

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));
        
        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitComplexObjectParameter(TestService service)
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


        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitComplexObjectParameter(TestService service)
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

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetImplicitDictionaryParameter(TestService service)
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
        
        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldSetExplicitDictionaryParameter(TestService service)
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

        // Act

        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));
        
        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldThrowExceptionIfExceptionParameterValueCannotBeCastToSchemeParameterType(TestService service)
    {
        // Arrange
        
        var name = Guid.NewGuid().ToString();

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(Exception));
        var value = "InvalidValue";

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new SetProcessParameterRequest(processId, name, value)));

        // Assert

        StringAssert.Contains(exception.Message, $"Unable to deserialize process parameter '{name}' to type '{typeof(Exception).FullName}'");
        Assert.AreEqual(exception.ErrorCode, 500);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldThrowExceptionIfBoolParameterValueCannotBeCastToSchemeParameterType(TestService service)
    {
        // Arrange
        
        var name = Guid.NewGuid().ToString();

        var processId = await CreateProcessWithExplicitParameterAsync(service, name, typeof(bool));
        var value = "InvalidValue";

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new SetProcessParameterRequest(processId, name, value)));

        // Assert

        StringAssert.Contains(exception.Message, $"Unable to deserialize process parameter '{name}' to type '{typeof(bool).FullName}'");
        Assert.AreEqual(exception.ErrorCode, 500);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldThrowExceptionIfSystemParameterSet(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "ProcessId";
        var value = Guid.NewGuid();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new SetProcessParameterRequest(processId, name, value)));

        // Assert

        StringAssert.Contains(exception.Message, $"Attempted to set system parameter '{name}'");
        Assert.AreEqual(exception.ErrorCode, 500);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldRemoveParameterIfNullValueSet(TestService service)
    {
        // Arrange

        var processId = await CreateProcessAsync(service);
        var name = "GuidParameter";
        var value = Guid.NewGuid();

        // Act
       
        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, value));

        // Assert

        var result = await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        AssertParameter(value, result.Value);

        // Act

        // ReSharper disable once RedundantArgumentDefaultValue
        await service.Client.RpcInstance.WorkflowApiRpcSetProcessParameterAsync(new(processId, name, null));

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.Parameters.WorkflowApiDataProcessesParametersGetAsync(processId, name));

        // Assert

        StringAssert.Contains(exception.Message, $"Parameter with process id {processId} and name {name} not found");
        Assert.AreEqual(404, exception.ErrorCode);
    }
    
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldExecute_WhenPermissionAllowed(TestService service)
    {
        // Arrange
        
        var processId = await CreateProcessAsync(service);
        var parameterName = "PermittedParam";
        var parameterValue = Guid.NewGuid();

        // Act
        
        var setRequest = new SetProcessParameterRequest(processId, parameterName, parameterValue);
        await service.Client.ExclusivePermissions(c => c.RpcInstance, WorkflowApiOperationId.RpcSetProcessParameter).WorkflowApiRpcSetProcessParameterAsync(setRequest);

        // Assert
        
        var result = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, parameterName));
        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        var request = new SetProcessParameterRequest(Guid.NewGuid());
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () => await service.Client.ExclusivePermissions(c => c.RpcInstance, Array.Empty<string>()).WorkflowApiRpcSetProcessParameterAsync(request));

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
        string expectedJson = JsonConvert.SerializeObject(expected);
        string actualJson = JsonConvert.SerializeObject(actual);

        expectedJson = expectedJson.ToLowerInvariant();
        actualJson = actualJson.ToLowerInvariant();

        Assert.AreEqual(expectedJson, actualJson);
    }
}
