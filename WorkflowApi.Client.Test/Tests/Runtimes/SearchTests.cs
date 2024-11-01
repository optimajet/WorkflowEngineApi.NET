using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;
using Direction = WorkflowApi.Client.Model.Direction;
using FilterType = WorkflowApi.Client.Model.FilterType;
using RuntimeField = WorkflowApi.Client.Model.RuntimeField;

namespace WorkflowApi.Client.Test.Tests.Runtimes;

[TestClass]
public class SearchTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new RuntimeFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            RuntimeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new RuntimeFieldQuery(
            search: models.First().Id
        );

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        RuntimeHelper.AssertModels(models.First(), actual);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithRuntimeIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        var query = new RuntimeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, RuntimeField.RuntimeId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Id);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithLockEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().VarLock;

        var query = new RuntimeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, RuntimeField.Lock, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.VarLock);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithStatusEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Status;

        var query = new RuntimeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, RuntimeField.Status, expected.ToString())],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Status);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithRestorerIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().RestorerId;

        var query = new RuntimeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, RuntimeField.RestorerId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.RestorerId);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithNextTimerTimeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().NextTimerTime;

        var query = new RuntimeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, RuntimeField.NextTimerTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.NextTimerTime);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithNextServiceTimerTimeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().NextServiceTimerTime;

        var query = new RuntimeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, RuntimeField.NextServiceTimerTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.NextServiceTimerTime);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithLastAliveSignalEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().LastAliveSignal;

        var query = new RuntimeFieldQuery
        {
            Filters = [new (FilterType.Equal, null, RuntimeField.LastAliveSignal, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.LastAliveSignal);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<RuntimeFieldFilter> filters = [
            new (FilterType.Equal, null, RuntimeField.RuntimeId, expected.Id),
            new (FilterType.Equal, null, RuntimeField.Lock, expected.VarLock),
            new (FilterType.Equal, null, RuntimeField.Status, expected.Status.ToString()),
            new (FilterType.Equal, null, RuntimeField.RestorerId, expected.RestorerId),
            new (FilterType.Equal, null, RuntimeField.NextTimerTime, expected.NextTimerTime),
            new (FilterType.Equal, null, RuntimeField.NextServiceTimerTime, expected.NextServiceTimerTime),
            new (FilterType.Equal, null, RuntimeField.LastAliveSignal, expected.LastAliveSignal),
        ];

        // Act

        var query = new RuntimeFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        RuntimeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<RuntimeFieldFilter> filters = [
            new (FilterType.Equal, null, RuntimeField.RuntimeId, expected.Id),
            new (FilterType.Equal, null, RuntimeField.Lock, expected.VarLock),
            new (FilterType.Equal, null, RuntimeField.Status, expected.Status.ToString()),
            new (FilterType.Equal, null, RuntimeField.RestorerId, expected.RestorerId),
            new (FilterType.Equal, null, RuntimeField.NextTimerTime, expected.NextTimerTime),
            new (FilterType.Equal, null, RuntimeField.NextServiceTimerTime, expected.NextServiceTimerTime),
            new (FilterType.Equal, null, RuntimeField.LastAliveSignal, expected.LastAliveSignal),
        ];

        // Act

        var query = new RuntimeFieldQuery(filters: [new RuntimeFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        RuntimeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(20);

        await repository.CreateAsync(models);

        List<RuntimeFieldFilter> filters = [
            new (FilterType.Equal, null, RuntimeField.RuntimeId, models[0].Id),
            new (FilterType.Equal, null, RuntimeField.Lock, models[1].VarLock),
            // new (FilterType.Equal, null, RuntimeField.Status, models[2].Status),
            new (FilterType.Equal, null, RuntimeField.RestorerId, models[2].RestorerId),
            new (FilterType.Equal, null, RuntimeField.NextTimerTime, models[3].NextTimerTime),
            new (FilterType.Equal, null, RuntimeField.NextServiceTimerTime, models[4].NextServiceTimerTime),
            new (FilterType.Equal, null, RuntimeField.LastAliveSignal, models[5].LastAliveSignal),
        ];

        // Act

        var query = new RuntimeFieldQuery(filters: [new RuntimeFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            RuntimeHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(20);

        await repository.CreateAsync(models);

        List<RuntimeFieldFilter> filters = [
            new (FilterType.Equal, null, RuntimeField.RuntimeId, models[0].Id),
            new (FilterType.Equal, null, RuntimeField.Lock, models[1].VarLock),
            // new (FilterType.Equal, null, RuntimeField.Status, models[2].Status),
            new (FilterType.Equal, null, RuntimeField.RestorerId, models[2].RestorerId),
            new (FilterType.Equal, null, RuntimeField.NextTimerTime, models[3].NextTimerTime),
            new (FilterType.Equal, null, RuntimeField.NextServiceTimerTime, models[4].NextServiceTimerTime),
            new (FilterType.Equal, null, RuntimeField.LastAliveSignal, models[5].LastAliveSignal),
        ];

        var notFilters = filters.Select(f => new RuntimeFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new RuntimeFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            Assert.IsNull(model);
        }
    }

    [ClientTest(ProviderName.Mongo, ProviderName.Mssql, ProviderName.Mysql, ProviderName.Oracle, ProviderName.Sqlite)]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(30);

        await repository.CreateAsync(models);

        var filter = new RuntimeFieldFilter(FilterType.In, null, RuntimeField.RuntimeId, models.Select(m => m.Id));
        var sort = new RuntimeFieldSort(RuntimeField.RuntimeId, Direction.Asc);

        // Act

        var queryPage1 = new RuntimeFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchRuntimesAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new RuntimeFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchRuntimesAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new RuntimeFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchRuntimesAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

        RuntimeModelGetCollectionResponse[] pages = [page1, page2, page3];

        // Assert

        foreach (var runtimeModel in pages.SelectMany(p => p.Collection).ToList())
        {
            Console.WriteLine($"{runtimeModel.Id}; {runtimeModel.Status}; {runtimeModel.VarLock}; {runtimeModel.RestorerId}; {runtimeModel.NextTimerTime}; {runtimeModel.NextServiceTimerTime}; {runtimeModel.LastAliveSignal}");
        }

        var idsCount = pages.SelectMany(p => p.Collection).Select(c => c.Id).Distinct().Count();

        Assert.AreEqual(30, idsCount);

        foreach (var page in pages)
        {
            Assert.AreEqual(30, page.Total);
            Assert.AreEqual(10, page.Collection.Count);

            foreach (var model in page.Collection)
            {
                var expected = models.First(x => x.Id == model.Id);
                RuntimeHelper.AssertModels(expected, model);
            }
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = await RuntimeHelper.ExclusiveSearchPermissionsApi(service);

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new RuntimeFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchRuntimesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            RuntimeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await RuntimeHelper.NoPermissionsApi(service);

        var query = new RuntimeFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchRuntimesAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}