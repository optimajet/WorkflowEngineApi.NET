using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class GetCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetGuidParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = typeof(Guid).ToString();

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", Guid.NewGuid()),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", Guid.NewGuid()),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", Guid.NewGuid())
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = typeof(string).ToString();

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", "String_1"),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", "String_2"),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", "String_3")
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetIntParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = typeof(int).ToString();

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", 1),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", 2),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", 3)
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDateTimeParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = typeof(DateTime).ToString();

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", DateTime.UtcNow.AddDays(-1)),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", DateTime.UtcNow),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", DateTime.UtcNow.AddDays(1))
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetBoolParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = typeof(bool).ToString();

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", true),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", false),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", true)
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetByteArrayParametersInCollection(TestService service)
    {
        // Arrange
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = "byteArray";

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new byte[] {1, 2, 3}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new byte[] {4, 5, 6}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new byte[] {7, 8, 9})
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringListParametersInCollection(TestService service)
    {
        // Arrange
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = "List<string>";

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new List<string> {"One", "Two"}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new List<string> {"Three", "Four"}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new List<string> {"Five", "Six"})
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetComplexParametersInCollection(TestService service)
    {
        // Arrange
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = typeof(ComplexTestObject).ToString();

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}",
                new ComplexTestObject {Prop1 = "A", Prop2 = 1, Nested = new NestedObject {SubProp1 = true, SubProp2 = [1]}}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}",
                new ComplexTestObject {Prop1 = "B", Prop2 = 2, Nested = new NestedObject {SubProp1 = false, SubProp2 = [2, 3]}}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}",
                new ComplexTestObject {Prop1 = "C", Prop2 = 3, Nested = new NestedObject {SubProp1 = true, SubProp2 = [3, 4, 5]}})
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDictionaryParametersInCollection(TestService service)
    {
        // Arrange
        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var type = "Dictionary<string, int>";

        var models = new[]
        {
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new Dictionary<string, int> {["A"] = 1}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new Dictionary<string, int> {["B"] = 2}),
            GlobalParameterHelper.CreateModel(type, $"{type}_Param_{Guid.NewGuid()}", new Dictionary<string, int> {["C"] = 3})
        };

        await repository.CreateAsync(models);

        // Act

        var api = service.Client.GlobalParameters;
        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(type);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            GlobalParameterHelper.AssertModels(model, actual, true);
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

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            GlobalParameterHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(search: models.First().Name);

        TestLogger.LogApiCalled(new { search = models.First().Name }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        GlobalParameterHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<GlobalParameterFieldFilter> filters = [
            new (FilterType.Equal, null, GlobalParameterField.Id, expected.Id),
            new (FilterType.Equal, null, GlobalParameterField.Type, expected.Type),
            new (FilterType.Equal, null, GlobalParameterField.Name, expected.Name),
            new (FilterType.Equal, null, GlobalParameterField.Value, expected.Value),
        ];

        // Act

        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(filters: filters);

        TestLogger.LogApiCalled(new { filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        GlobalParameterHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<GlobalParameterFieldFilter> filters = [
            new (FilterType.Equal, null, GlobalParameterField.Id, expected.Id),
            new (FilterType.Equal, null, GlobalParameterField.Type, expected.Type),
            new (FilterType.Equal, null, GlobalParameterField.Name, expected.Name),
            new (FilterType.Equal, null, GlobalParameterField.Value, expected.Value),
        ];

        List<GlobalParameterFieldFilter> andFilters = [new GlobalParameterFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(filters: andFilters);

        TestLogger.LogApiCalled(new { filters = andFilters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        GlobalParameterHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(20);

        await repository.CreateAsync(models);

        List<GlobalParameterFieldFilter> filters = [
            new (FilterType.Equal, null, GlobalParameterField.Id, models[0].Id),
            new (FilterType.Equal, null, GlobalParameterField.Type, models[1].Type),
            new (FilterType.Equal, null, GlobalParameterField.Name, models[2].Name),
            new (FilterType.Equal, null, GlobalParameterField.Value, models[3].Value),
        ];

        List<GlobalParameterFieldFilter> orFilters = [new GlobalParameterFieldFilter(FilterType.Or, filters)];

        // Act

        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(filters: orFilters);

        TestLogger.LogApiCalled(new { filters = orFilters }, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            GlobalParameterHelper.AssertModels(expectedModel, model, true);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(20);

        await repository.CreateAsync(models);

        List<GlobalParameterFieldFilter> filters = [
            new (FilterType.Equal, null, GlobalParameterField.Id, models[0].Id),
            new (FilterType.Equal, null, GlobalParameterField.Type, models[1].Type),
            new (FilterType.Equal, null, GlobalParameterField.Name, models[2].Name),
            new (FilterType.Equal, null, GlobalParameterField.Value, models[3].Value),
        ];

        var notFilters = filters.Select(f => new GlobalParameterFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(filters: notFilters);

        TestLogger.LogApiCalled(new { filters = notFilters }, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            Assert.IsNull(model);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(30);

        await repository.CreateAsync(models);

        List<GlobalParameterFieldFilter> filters = [new GlobalParameterFieldFilter(FilterType.In, null, GlobalParameterField.Id, models.Select(m => m.Id))];
        List<GlobalParameterFieldSort> sorts = [new GlobalParameterFieldSort(GlobalParameterField.Id, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 0 }, page1);

        var page2 = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 10 }, page2);

        var page3 = await api.WorkflowApiDataGlobalParametersGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 20 }, page3);

        GlobalParameterModelGetCollectionResponse[] pages = [page1, page2, page3];

        // Assert

        var idsCount = pages.SelectMany(p => p.Collection).Select(c => c.Id).Distinct().Count();

        Assert.AreEqual(30, idsCount);

        foreach (var page in pages)
        {
            Assert.AreEqual(30, page.Total);
            Assert.AreEqual(10, page.Collection.Count);

            foreach (var model in page.Collection)
            {
                var expected = models.First(x => x.Id == model.Id);
                GlobalParameterHelper.AssertModels(expected, model, true);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "get-collection");

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataGlobalParametersGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            GlobalParameterHelper.AssertModels(models[i], actual, true);
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
            async () => await api.WorkflowApiDataGlobalParametersGetCollectionAsync()
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}