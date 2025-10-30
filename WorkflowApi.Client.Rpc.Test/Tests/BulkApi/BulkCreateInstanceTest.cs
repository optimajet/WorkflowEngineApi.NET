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
using BulkTaskState = WorkflowApi.Client.Model.BulkTaskState;
using ParameterPurpose = OptimaJet.Workflow.Core.Model.ParameterPurpose;

namespace WorkflowApi.Client.Rpc.Test.Tests.BulkApi;

[TestClass]
public class BulkCreateInstanceTest
{
    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldCreateInstances(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var request = new BulkCreateInstanceRequest([]);

        for (int i = 0; i < 3; i++)
        {
            request.CreateInstanceParamsList.Add(new()
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
                    new("Sixth", new List<string> {"One", "Two", "Three"}, ParameterPurposeWithoutSystem.Persistence),
                    new("Seventh", new ComplexTestObject() { Prop1 = "Test", Prop2 = 42, Nested = new NestedObject { SubProp1 = true, SubProp2 = new List<int> { 1, 2, 3 } } }, ParameterPurposeWithoutSystem.Persistence)
                ]
            });
        }

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkCreateInstanceAsync(request);

        // Assert

        foreach (var (processId, createInstanceParamsItem) in request.CreateInstanceParamsList.Select(x => (x.ProcessId, x)))
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNotNull(result.Result);
            Assert.IsNull(result.Exception);

            var processInstanceResult = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new(processId));
            var processInstance = processInstanceResult.ProcessInstance;

            Assert.IsNotNull(processInstance);
            Assert.AreEqual(processId, processInstance.ProcessId);
            Assert.AreEqual(schemeCode, processInstance.SchemeCode);
            Assert.AreEqual(createInstanceParamsItem.CalendarName, processInstance.CalendarName);
            Assert.AreEqual(service.TenantId, processInstance.TenantId);

            foreach (var parameter in createInstanceParamsItem.InitialProcessParameters)
            {
                var parameterResult = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, parameter.Name));

                AssertParameter(parameter.Value, parameterResult.Value);
            }
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldCreateInstances_WithExplicitParameters(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial()
            .CreateParameter("First", typeof(string), ParameterPurpose.Persistence)
            .CreateParameter("Second", typeof(int), ParameterPurpose.Persistence)
            .CreateParameter("Third", typeof(DateTime), ParameterPurpose.Persistence)
            .CreateParameter("Fourth", typeof(bool), ParameterPurpose.Persistence)
            .CreateParameter("Fifth", typeof(byte[]), ParameterPurpose.Persistence)
            .CreateParameter("Sixth", typeof(List<string>), ParameterPurpose.Persistence)
            .CreateParameter("Seventh", typeof(ComplexTestObject), ParameterPurpose.Persistence);

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var request = new BulkCreateInstanceRequest([]);

        for (int i = 0; i < 3; i++)
        {
            request.CreateInstanceParamsList.Add(new()
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
                    new("Sixth", new List<string> {"One", "Two", "Three"}, ParameterPurposeWithoutSystem.Persistence),
                    new("Seventh", new ComplexTestObject() { Prop1 = "Test", Prop2 = 42, Nested = new NestedObject { SubProp1 = true, SubProp2 = new List<int> { 1, 2, 3 } } }, ParameterPurposeWithoutSystem.Persistence)
                ]
            });
        }

        // Act

        var response = await service.Client.RpcBulk.WorkflowApiRpcBulkCreateInstanceAsync(request);

        // Assert

        foreach (var (processId, createInstanceParamsItem) in request.CreateInstanceParamsList.Select(x => (x.ProcessId, x)))
        {
            var result = response.FirstOrDefault(pair => pair.Key == processId.ToString()).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(BulkTaskState.Completed, result.State);
            Assert.IsNotNull(result.Result);
            Assert.IsNull(result.Exception);

            var processInstanceResult = await service.Client.RpcInstance.WorkflowApiRpcGetProcessInstanceAsync(new(processId));
            var processInstance = processInstanceResult.ProcessInstance;

            Assert.IsNotNull(processInstance);
            Assert.AreEqual(processId, processInstance.ProcessId);
            Assert.AreEqual(schemeCode, processInstance.SchemeCode);
            Assert.AreEqual(createInstanceParamsItem.CalendarName, processInstance.CalendarName);
            Assert.AreEqual(service.TenantId, processInstance.TenantId);

            foreach (var parameter in createInstanceParamsItem.InitialProcessParameters)
            {
                var parameterResult = await service.Client.RpcInstance.WorkflowApiRpcGetProcessParameterAsync(new(processId, parameter.Name));

                AssertParameter(parameter.Value, parameterResult.Value);
            }
        }
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn500_WithSystemParameter(TestService service)
    {
        // Arrange

        var schemeCode = Guid.NewGuid().ToString();

        var builder = ProcessDefinitionBuilder
            .Create(schemeCode)
            .CreateActivity("Activity1").Initial();

        await service.Client.Schemes.CreateSchemeFromBuilderAsync(builder);

        var request = new BulkCreateInstanceRequest([]);

        for (int i = 0; i < 3; i++)
        {
            request.CreateInstanceParamsList.Add(new()
            {
                ProcessId = Guid.NewGuid(),
                SchemeCode = schemeCode,
                IdentityId = Guid.NewGuid().ToString(),
                ImpersonatedIdentityId = Guid.NewGuid().ToString(),
                CalendarName = Guid.NewGuid().ToString(),
                InitialProcessParameters =
                [
                    new("ProcessId", "Value", ParameterPurposeWithoutSystem.Persistence)
                ]
            });
        }

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.RpcBulk.WorkflowApiRpcBulkCreateInstanceAsync(request)
        );

        // Assert

        Assert.AreEqual(500, exception.ErrorCode);
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

        var request = new BulkCreateInstanceRequest([new (schemeCode, Guid.NewGuid())]);

        // Act

        var result = await service.Client.ExclusivePermissions(c => c.RpcBulk, WorkflowApiOperationId.RpcBulkCreateInstance).WorkflowApiRpcBulkCreateInstanceAsync(request);

        // Assert

        Assert.IsNotNull(result);
    }

    [ClientTest(HostId.RpcHost)]
    [TestMethod]
    public async Task ShouldReturn403_WhenPermissionDenied(TestService service)
    {
        // Arrange

        var request = new BulkCreateInstanceRequest([]);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await service.Client.ExclusivePermissions(c => c.RpcBulk, Array.Empty<string>()).WorkflowApiRpcBulkCreateInstanceAsync(request)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
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
