using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class CreateTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateGuidParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "GuidParam";
        var value = Guid.NewGuid();
        var model = ParameterHelper.CreateModel(processId, name, value);

        // Act
        
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringParameter(TestService service)
    {
        // Arrange

        var processId = Guid.NewGuid();
        var name = "StringParam";
        var value = "TestString";
        var model = ParameterHelper.CreateModel(processId, name, value);

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateIntParameter(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var name = "IntParam";
        var value = 123;
        var model = ParameterHelper.CreateModel(processId, name, value);

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDateTimeParameter(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var name = "DateTimeParam";
        var value = DateTime.UtcNow;
        var model = ParameterHelper.CreateModel(processId, name, value);

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateBoolParameter(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var name = "BoolParam";
        var value = true;
        var model = ParameterHelper.CreateModel(processId, name, value);

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateByteArrayParameter(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var name = "ByteArrayParam";
        var value = new byte[] {1, 2, 3};
        var model = ParameterHelper.CreateModel(processId, name, value);

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringListParameter(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var name = "ListParam";
        var value = new List<string> {"One", "Two", "Three"};
        var model = ParameterHelper.CreateModel(processId, name, value);

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateComplexObjectParameter(TestService service)
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

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDictionaryParameter(TestService service)
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

        // Act

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(model.ProcessId));
        }
        
        var api = service.Client.Parameters;
        var request = ParameterHelper.CreateRequest(model);
        var result = await api.WorkflowApiDataProcessesParametersCreateAsync(model.ProcessId, model.Name, request);
        
        TestLogger.LogApiCalled(new {model.ProcessId, model.Name, request}, result);

        // Assert

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var dbModel = await repository.GetAsync(model.ProcessId, model.Name);

        ParameterHelper.AssertModels(model, dbModel);
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

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        foreach (var model in models)
        {
            var request = ParameterHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataProcessesParametersCreateAsync(processId, model.Name, request);

            TestLogger.LogApiCalled(new { processId, model.Name, request }, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "create");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        foreach (var model in models)
        {
            var request = ParameterHelper.CreateRequest(model);
            var result = await api.WorkflowApiDataProcessesParametersCreateAsync(processId, model.Name, request);

            TestLogger.LogApiCalled(new { processId, model.Name, request }, result);
        }

        // Assert

        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersCreateAsync(Guid.NewGuid(), Guid.NewGuid().ToString(), ParameterHelper.CreateRequest(ParameterHelper.Generate(Guid.NewGuid())))
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost, ExcludeProviders = [ProviderName.Mongo])] //No constraints
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var model = ParameterHelper.Generate(processId);

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        await repository.CreateAsync(model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersCreateAsync(processId, model.Name, ParameterHelper.CreateRequest(model))
        );

        // Assert

        Assert.AreEqual(500, exception.ErrorCode);
    }
}