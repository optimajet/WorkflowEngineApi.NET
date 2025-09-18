using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class CreateCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateGuidGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(Guid).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", Guid.NewGuid()),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", Guid.NewGuid())
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(string).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(string).ToString(), $"{type}_Param_{Guid.NewGuid()}", "Value1"),
            GlobalParameterHelper.CreateModel(typeof(string).ToString(), $"{type}_Param_{Guid.NewGuid()}", "Value2")
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateIntGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(int).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(int).ToString(), $"{type}_Param_{Guid.NewGuid()}", 1),
            GlobalParameterHelper.CreateModel(typeof(int).ToString(), $"{type}_Param_{Guid.NewGuid()}", 2)
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDateTimeGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(DateTime).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(DateTime).ToString(), $"{type}_Param_{Guid.NewGuid()}", DateTime.UtcNow),
            GlobalParameterHelper.CreateModel(typeof(DateTime).ToString(), $"{type}_Param_{Guid.NewGuid()}", DateTime.UtcNow.AddDays(1))
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateBoolGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(bool).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(bool).ToString(), $"{type}_Param_{Guid.NewGuid()}", true),
            GlobalParameterHelper.CreateModel(typeof(bool).ToString(), $"{type}_Param_{Guid.NewGuid()}", false)
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateByteArrayGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(byte[]).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(byte[]).ToString(), $"{type}_Param_{Guid.NewGuid()}", new byte[] {1, 2, 3}),
            GlobalParameterHelper.CreateModel(typeof(byte[]).ToString(), $"{type}_Param_{Guid.NewGuid()}", new byte[] {4, 5, 6})
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateStringListGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(List<string>).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(List<string>).ToString(), $"{type}_Param_{Guid.NewGuid()}",
                new List<string> {"One", "Two"}),
            GlobalParameterHelper.CreateModel(typeof(List<string>).ToString(), $"{type}_Param_{Guid.NewGuid()}",
                new List<string> {"Three", "Four"})
        };
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateComplexObjectGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(ComplexTestObject).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(ComplexTestObject).ToString(), $"{type}_Param_{Guid.NewGuid()}", new ComplexTestObject
            {
                Prop1 = "NestedValue1",
                Prop2 = 123,
                Nested = new NestedObject
                {
                    SubProp1 = true,
                    SubProp2 = [1, 2]
                }
            }),
            GlobalParameterHelper.CreateModel(typeof(ComplexTestObject).ToString(), $"{type}_Param_{Guid.NewGuid()}", new ComplexTestObject
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
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldCreateDictionaryGlobalParametersCollection(TestService service)
    {
        // Arrange
        
        var type = typeof(Dictionary<string, int>).ToString();
        var models = new[]
        {
            GlobalParameterHelper.CreateModel(typeof(Dictionary<string, int>).ToString(), $"{type}_Param_{Guid.NewGuid()}",
                new Dictionary<string, int> {["Key1"] = 1, ["Key2"] = 2}),
            GlobalParameterHelper.CreateModel(typeof(Dictionary<string, int>).ToString(), $"{type}_Param_{Guid.NewGuid()}",
                new Dictionary<string, int> {["Key3"] = 3, ["Key4"] = 4})
        };
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;

        // Act
        
        var api = service.Client.GlobalParameters;
        var requests = GlobalParameterHelper.CreateRequests(models);
        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        // Assert
        
        Assert.AreEqual(models.Length, createResult.CreatedCount);
        
        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }
    
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);
        var requests = GlobalParameterHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        TestLogger.LogApiCalled(new {requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "create-collection");

        var models = GlobalParameterHelper.Generate(3);
        var requests = GlobalParameterHelper.CreateRequests(models);

        // Act

        var createResult = await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests);

        TestLogger.LogApiCalled(new {requests}, createResult);

        // Assert

        Assert.AreEqual(models.Length, createResult.CreatedCount);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Type, model.Name);
            GlobalParameterHelper.AssertModels(model, result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await GlobalParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersCreateCollectionAsync([])
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ConflictTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var model = GlobalParameterHelper.Generate();

        await repository.CreateAsync(model);

        var requests = GlobalParameterHelper.CreateRequests(model);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersCreateCollectionAsync(requests)
        );

        // Assert

        Assert.AreEqual(500, exception.ErrorCode);
    }
}