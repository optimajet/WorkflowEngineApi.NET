using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using OptimaJet.Workflow.Core;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Parameters;

[TestClass]
public class SearchTests
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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Equal, field: ParameterField.ProcessId, value: processId)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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

        var query = new ParameterFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.ProcessId == models[i].ProcessId && x.Name == models[i].Name);
            ParameterHelper.AssertModels(models[i], actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
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

        var query = new ParameterFieldQuery(
            search: models.First().Name
        );

        // Act

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        ParameterHelper.AssertModels(models.First(), actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
    }

    [ClientTest(HostId.DataHost, ExcludeProviders = [ProviderName.Mongo])] // Mongo does not support parameter id field
    [TestMethod]
    public async Task ExecuteWithIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        // Act

        var query = new ParameterFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ParameterField.Id, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
    public async Task ExecuteWithProcessIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ProcessId;

        // Act

        var query = new ParameterFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ParameterField.ProcessId, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ProcessId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithParameterNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Name;

        // Act

        var query = new ParameterFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ParameterField.ParameterName, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Value;

        // Act

        var query = new ParameterFieldQuery
        {
            Filters = [new (FilterType.Equal, null, ParameterField.Value, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var models = Enumerable.Range(0, 20).Select(_ => ParameterHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, expected.Id),
            new (FilterType.Equal, null, ParameterField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, expected.Name),
            new (FilterType.Equal, null, ParameterField.Value, expected.Value),
        ];

        // Act

        var query = new ParameterFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var models = Enumerable.Range(0, 20).Select(_ => ParameterHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, expected.Id),
            new (FilterType.Equal, null, ParameterField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, expected.Name),
            new (FilterType.Equal, null, ParameterField.Value, expected.Value),
        ];

        // Act

        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var models = Enumerable.Range(0, 20).Select(_ => ParameterHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, models[0].Id),
            new (FilterType.Equal, null, ParameterField.ProcessId, models[0].ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, models[1].Name),
            new (FilterType.Equal, null, ParameterField.Value, models[2].Value),
        ];

        // Act

        var query = new ParameterFieldQuery(filters: [new ParameterFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var models = Enumerable.Range(0, 20).Select(_ => ParameterHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<ParameterFieldFilter> filters = [
            // new (FilterType.Equal, null, ParameterField.Id, models[0].Id),
            new (FilterType.Equal, null, ParameterField.ProcessId, models[0].ProcessId),
            new (FilterType.Equal, null, ParameterField.ParameterName, models[1].Name),
            new (FilterType.Equal, null, ParameterField.Value, models[2].Value),
        ];

        var notFilters = filters.Select(f => new ParameterFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new ParameterFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

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
        var repository = service.Repository.Parameters;
        var api = service.Client.Parameters;

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        var filter = new ParameterFieldFilter(FilterType.Equal, null, ParameterField.ProcessId, processId);
        var sort = new ParameterFieldSort(ParameterField.ParameterName, Direction.Asc);

        // Act

        var queryPage1 = new ParameterFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchProcessesParametersAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new ParameterFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchProcessesParametersAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new ParameterFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchProcessesParametersAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

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
        var api = await ParameterHelper.ExclusiveSearchPermissionsApi(service);

        var processId = Guid.NewGuid();
        var models = ParameterHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new ParameterFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.ProcessId == models[i].ProcessId && x.Name == models[i].Name);
            ParameterHelper.AssertModels(models[i], actual, service.TenantOptions.DataProviderId != PersistenceProviderId.Mongo);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ParameterHelper.NoPermissionsApi(service);

        var query = new ParameterFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchProcessesParametersAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}