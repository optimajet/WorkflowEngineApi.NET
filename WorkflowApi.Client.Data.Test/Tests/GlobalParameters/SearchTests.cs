using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class SearchTests
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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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
        var result = await api.WorkflowApiSearchGlobalParametersAsync(new ());

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

        var query = new GlobalParameterFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var query = new GlobalParameterFieldQuery(
            search: models.First().Name
        );

        // Act

        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        GlobalParameterHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        var query = new GlobalParameterFieldQuery
        {
            Filters = [new GlobalParameterFieldFilter(FilterType.Equal, null, GlobalParameterField.Id, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Id);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithTypeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Type;

        var query = new GlobalParameterFieldQuery
        {
            Filters = [new GlobalParameterFieldFilter(FilterType.Equal, null, GlobalParameterField.Type, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Type);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Name;

        var query = new GlobalParameterFieldQuery
        {
            Filters = [new GlobalParameterFieldFilter(FilterType.Equal, null, GlobalParameterField.Name, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Name);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithValueEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Value;

        var query = new GlobalParameterFieldQuery
        {
            Filters = [new GlobalParameterFieldFilter(FilterType.Equal, null, GlobalParameterField.Value, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Value);
        }
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

        var query = new GlobalParameterFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        // Act

        var query = new GlobalParameterFieldQuery(filters: [new GlobalParameterFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        // Act

        var query = new GlobalParameterFieldQuery(filters: [new GlobalParameterFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var query = new GlobalParameterFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var filter = new GlobalParameterFieldFilter(FilterType.In, null, GlobalParameterField.Id, models.Select(m => m.Id));
        var sort = new GlobalParameterFieldSort(GlobalParameterField.Id, Direction.Asc);

        // Act

        var queryPage1 = new GlobalParameterFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchGlobalParametersAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new GlobalParameterFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchGlobalParametersAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new GlobalParameterFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchGlobalParametersAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

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
        var api = await GlobalParameterHelper.ExclusiveSearchPermissionsApi(service);

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new GlobalParameterFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchGlobalParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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
            async () => await api.WorkflowApiSearchGlobalParametersAsync(new GlobalParameterFieldQuery())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}