using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class GetCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetMultipleTypesAndValuesInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        
        var models = Enumerable.Range(0, 3).SelectMany(i => new[]
        {
            ParameterHelper.CreateModel(processId, $"GuidParam_{i}", Guid.NewGuid()),
            ParameterHelper.CreateModel(processId, $"StringParam_{i}", $"TestString_{i}"),
            ParameterHelper.CreateModel(processId, $"IntParam_{i}", 100 + i),
            ParameterHelper.CreateModel(processId, $"BoolParam_{i}", i % 2 == 0),
            ParameterHelper.CreateModel(processId, $"DateTimeParam_{i}", DateTime.UtcNow.AddDays(-i)),
            ParameterHelper.CreateModel(processId, $"ByteArrayParam_{i}", new[] {(byte) i, (byte) (i + 1), (byte) (i + 2)}),
            ParameterHelper.CreateModel(processId, $"ListParam_{i}", new List<string> {$"One_{i}", $"Two_{i}", $"Three_{i}"}),
            ParameterHelper.CreateModel(processId, $"DictParam_{i}", new Dictionary<string, int>
            {
                [$"Key1_{i}"] = i,
                [$"Key2_{i}"] = i + 1
            }),
            ParameterHelper.CreateModel(processId, $"ComplexParam_{i}", new ComplexTestObject
            {
                Prop1 = $"Nested_{i}",
                Prop2 = 1000 + i,
                Nested = new NestedObject
                {
                    SubProp1 = i % 2 == 1,
                    SubProp2 = [i, i + 1, i + 2]
                }
            })
        }).ToArray();

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetGuidParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "GuidParam_1", Guid.NewGuid()),
            ParameterHelper.CreateModel(processId, "GuidParam_2", Guid.NewGuid()),
            ParameterHelper.CreateModel(processId, "GuidParam_3", Guid.NewGuid())
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "StringParam_1", "String_1"),
            ParameterHelper.CreateModel(processId, "StringParam_2", "String_2"),
            ParameterHelper.CreateModel(processId, "StringParam_3", "String_3")
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetIntParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "IntParam_1", 1),
            ParameterHelper.CreateModel(processId, "IntParam_2", 2),
            ParameterHelper.CreateModel(processId, "IntParam_3", 3)
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDateTimeParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "DateTimeParam_1", DateTime.UtcNow.AddDays(-1)),
            ParameterHelper.CreateModel(processId, "DateTimeParam_2", DateTime.UtcNow),
            ParameterHelper.CreateModel(processId, "DateTimeParam_3", DateTime.UtcNow.AddDays(1))
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetBoolParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "BoolParam_1", true),
            ParameterHelper.CreateModel(processId, "BoolParam_2", false),
            ParameterHelper.CreateModel(processId, "BoolParam_3", true)
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetByteArrayParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "ByteArrayParam_1", new byte[] {1, 2, 3}),
            ParameterHelper.CreateModel(processId, "ByteArrayParam_2", new byte[] {4, 5, 6}),
            ParameterHelper.CreateModel(processId, "ByteArrayParam_3", new byte[] {7, 8, 9})
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetStringListParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "ListParam_1", new List<string> {"One", "Two"}),
            ParameterHelper.CreateModel(processId, "ListParam_2", new List<string> {"Three", "Four"}),
            ParameterHelper.CreateModel(processId, "ListParam_3", new List<string> {"Five", "Six"})
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetComplexParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "ComplexParam_1",
                new ComplexTestObject {Prop1 = "A", Prop2 = 1, Nested = new NestedObject {SubProp1 = true, SubProp2 = [1]}}),
            ParameterHelper.CreateModel(processId, "ComplexParam_2",
                new ComplexTestObject {Prop1 = "B", Prop2 = 2, Nested = new NestedObject {SubProp1 = false, SubProp2 = [2, 3]}}),
            ParameterHelper.CreateModel(processId, "ComplexParam_3",
                new ComplexTestObject {Prop1 = "C", Prop2 = 3, Nested = new NestedObject {SubProp1 = true, SubProp2 = [3, 4, 5]}})
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ShouldGetDictionaryParametersInCollection(TestService service)
    {
        // Arrange
        
        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var processId = Guid.NewGuid();
        var models = new[]
        {
            ParameterHelper.CreateModel(processId, "DictParam_1", new Dictionary<string, int> {["A"] = 1}),
            ParameterHelper.CreateModel(processId, "DictParam_2", new Dictionary<string, int> {["B"] = 2}),
            ParameterHelper.CreateModel(processId, "DictParam_3", new Dictionary<string, int> {["C"] = 3})
        };

        await repository.CreateAsync(models);

        // Act
        
        var api = service.Client.Parameters;
        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        // Assert
        
        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Name == model.Name);
            Assert.IsNotNull(actual);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
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

        await repository.CreateAsync(models);
        await repository.CreateAsync(ParameterHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.ProcessId == model.ProcessId && x.Name == model.Name);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, search: models.First().Name);

        TestLogger.LogApiCalled(new {processId, search = models.First().Name}, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        ParameterHelper.AssertModels(models.First(), actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, expected.Id),
            // new (FilterType.Equal, null, ParameterField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, expected.Name),
            new (FilterType.Equal, null, ParameterField.Value, expected.Value),
        ];

        // Act

        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new {processId, filters}, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ParameterHelper.AssertModels(expected, result.Collection.First(), service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, expected.Id),
            // new (FilterType.Equal, null, ParameterField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, expected.Name),
            new (FilterType.Equal, null, ParameterField.Value, expected.Value),
        ];

        List<ParameterFieldFilter> andFilter = [new ParameterFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, filters: andFilter);

        TestLogger.LogApiCalled(new {processId, filters = andFilter}, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ParameterHelper.AssertModels(expected, result.Collection.First(), service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, models[0].Id),
            // new (FilterType.Equal, null, ParameterField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, models[0].Name),
            new (FilterType.Equal, null, ParameterField.Value, models[1].Value),
        ];

        // Act

        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, filters: [new ParameterFieldFilter(FilterType.Or, filters)]);

        TestLogger.LogApiCalled(new {processId, filters}, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.ProcessId == model.ProcessId && x.Name == model.Name);
            ParameterHelper.AssertModels(expectedModel, model, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, models[0].Id),
            // new (FilterType.Equal, null, ParameterField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, models[0].Name),
            new (FilterType.Equal, null, ParameterField.Value, models[1].Value),
        ];

        var notFilters = filters.Select(f => new ParameterFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new {processId, filters = notFilters}, result);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, result.Total);

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
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        List<ParameterFieldSort> sorts = [new ParameterFieldSort(ParameterField.ParameterName, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 0}, page1);

        var page2 = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 10}, page2);

        var page3 = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 20}, page3);

        ParameterModelGetCollectionResponse[] pages = [page1, page2, page3];

        // Assert

        var idsCount = pages.SelectMany(p => p.Collection).Select(c => $"{c.ProcessId}{c.Name}").Distinct().Count();

        Assert.AreEqual(30, idsCount);

        foreach (var page in pages)
        {
            Assert.AreEqual(30, page.Total);
            Assert.AreEqual(10, page.Collection.Count);

            foreach (var model in page.Collection)
            {
                var expected = models.First(x => x.ProcessId == model.ProcessId && x.Name == model.Name);
                ParameterHelper.AssertModels(expected, model, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = await ParameterHelper.ExclusivePermissionsApi(service, "get-collection");

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);
        await repository.CreateAsync(ParameterHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesParametersGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.ProcessId == model.ProcessId && x.Name == model.Name);
            ParameterHelper.AssertModels(model, actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
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
            async () => await api.WorkflowApiDataProcessesParametersGetCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}