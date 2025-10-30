using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class CreateCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateGuidParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "GuidParam1", Guid.NewGuid()),
            ParameterHelper.CreateModel(processId, "GuidParam2", Guid.NewGuid())
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);

        // Assert
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "StringParam1", "Value1"),
            ParameterHelper.CreateModel(processId, "StringParam2", "Value2")
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }


        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);


        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateIntParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "IntParam1", 1),
            ParameterHelper.CreateModel(processId, "IntParam2", 2)
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }
        
        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);


        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDateTimeParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "DateTimeParam1", DateTime.UtcNow),
            ParameterHelper.CreateModel(processId, "DateTimeParam2", DateTime.UtcNow.AddDays(1))
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }


        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);


        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateBoolParametersCollection(TestService service)
    {
        // Arrange
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "BoolParam1", true),
            ParameterHelper.CreateModel(processId, "BoolParam2", false)
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }


        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);


        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateByteArrayParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "ByteArrayParam1", new byte[] {1, 2, 3}),
            ParameterHelper.CreateModel(processId, "ByteArrayParam2", new byte[] {4, 5, 6})
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }


        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);


        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringListParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "ListParam1", new List<string> {"One", "Two"}),
            ParameterHelper.CreateModel(processId, "ListParam2", new List<string> {"Three", "Four"})
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }


        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);


        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateComplexObjectParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "ComplexParam1", new ComplexTestObject
            {
                Prop1 = "NestedValue1",
                Prop2 = 123,
                Nested = new NestedObject
                {
                    SubProp1 = true,
                    SubProp2 = [1, 2]
                }
            }),
            ParameterHelper.CreateModel(processId, "ComplexParam2", new ComplexTestObject
            {
                Prop1 = "NestedValue2",
                Prop2 = 456,
                Nested = new NestedObject
                {
                    SubProp1 = false,
                    SubProp2 = [3, 4]
                }
            })
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }
        
        // Act
        
        var api = service.Client.Parameters;
        var requests = ParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);


        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDictionaryParametersCollection(TestService service)
    {
        // Arrange
        
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "DictParam1", new Dictionary<string, int>
            {
                ["Key1"] = 1,
                ["Key2"] = 2
            }),
            ParameterHelper.CreateModel(processId, "DictParam2", new Dictionary<string, int>
            {
                ["Key3"] = 3,
                ["Key4"] = 4
            })
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act
        
        var requests = ParameterHelper.CreateRequests(models);
        var api = service.Client.Parameters;
        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);
        
        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(processId, model.Name);
            ParameterHelper.AssertModels(model, result);
        }
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
        var requests = ParameterHelper.CreateRequests(models);

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);

        TestLogger.LogApiCalled(new {processId, requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

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
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "create-collection");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);
        var requests = ParameterHelper.CreateRequests(models);

        if (service.TenantOptions.PersistenceProviderId == PersistenceProviderId.Mongo)
        {
            await service.Repository.Processes.CreateAsync(new ProcessModel(processId));
        }

        // Act

        var createResult = await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests);

        TestLogger.LogApiCalled(new {processId, requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

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
            async () => await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(Guid.NewGuid(), [])
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

        await repository.CreateAsync(model);

        // Act

        var requests = ParameterHelper.CreateRequests(model);
        
        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesParametersCreateCollectionAsync(processId, requests)
        );

        // Assert

        Assert.AreEqual(500, exception.ErrorCode);
    }
}