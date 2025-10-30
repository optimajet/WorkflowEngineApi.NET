using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;
using WorkflowApi.Client.Model;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class GetTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetGuidParameter(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var name = "GuidParam";
        var value = Guid.NewGuid();
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "StringParam";
        var value = "TestString";
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetIntParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "IntParam";
        var value = 123;
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDateTimeParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "DateTimeParam";
        var value = DateTime.UtcNow;
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetBoolParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "BoolParam";
        var value = true;
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetByteArrayParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "ByteArrayParam";
        var value = new byte[] {1, 2, 3};
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringListParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "ListParam";
        var value = new List<string> {"One", "Two", "Three"};
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetComplexObjectParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "ComplexParam";
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
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDictionaryParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "DictParam";
        var value = new Dictionary<string, int>
        {
            ["Key1"] = 1,
            ["Key2"] = 2,
            ["Key3"] = 3
        };
        var model = ParameterHelper.CreateModel(processId, name, value);

        await using var context = service.Repository.Use();
        await service.Repository.Parameters.CreateAsync(model);

        // Act

        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, name);
        
        // Assert

        ParameterHelper.AssertModels(model, result, service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        List<ParameterModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, model.Name);

            TestLogger.LogApiCalled(new {processId, model.Name}, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            ParameterHelper.AssertModels(models[i], results[i], service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "get");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        List<ParameterModel> results = [];

        foreach (var model in models)
        {
            var result = await api.WorkflowApiDataProcessesParametersGetAsync(processId, model.Name);

            TestLogger.LogApiCalled(new {processId, model.Name}, result);

            results.Add(result);
        }

        // Assert

        for (int i = 0; i < models.Length; i++)
        {
            ParameterHelper.AssertModels(models[i], results[i], service.TenantOptions.PersistenceProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
            await api.WorkflowApiDataProcessesParametersGetAsync(Guid.NewGuid(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task NotFoundTest(TestService service)
    {
        // Arrange

        var api = service.Client.Parameters;

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
            await api.WorkflowApiDataProcessesParametersGetAsync(Guid.NewGuid(), Guid.NewGuid().ToString())
        );

        // Assert

        Assert.AreEqual(404, exception.ErrorCode);
    }
}