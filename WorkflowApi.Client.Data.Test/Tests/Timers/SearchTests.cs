using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Timers;

[TestClass]
public class SearchTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new TimerFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            TimerHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new TimerFieldQuery(
            search: models.First().Name
        );

        // Act

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        TimerHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        // Act

        var query = new TimerFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TimerField.Id, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

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
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ProcessId;

        // Act

        var query = new TimerFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TimerField.ProcessId, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

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
    public async Task ExecuteWithRootProcessIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().RootProcessId;

        // Act

        var query = new TimerFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TimerField.RootProcessId, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.RootProcessId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithNameEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Name;

        // Act

        var query = new TimerFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TimerField.Name, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

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
    public async Task ExecuteWithNextExecutionDateTimeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().NextExecutionDateTime;

        // Act

        var query = new TimerFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TimerField.NextExecutionDateTime, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.NextExecutionDateTime);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIgnoreEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Ignore;

        // Act

        var query = new TimerFieldQuery
        {
            Filters = [new (FilterType.Equal, null, TimerField.Ignore, expected)],
            Take = 100
        };

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Ignore);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var models = Enumerable.Range(0, 20).Select(_ => TimerHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, expected.Id),
            new (FilterType.Equal, null, TimerField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, expected.Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, expected.NextExecutionDateTime),
            new (FilterType.Equal, null, TimerField.Ignore, expected.Ignore),
        ];

        // Act

        var query = new TimerFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        TimerHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var models = Enumerable.Range(0, 20).Select(_ => TimerHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, expected.Id),
            new (FilterType.Equal, null, TimerField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, expected.Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, expected.NextExecutionDateTime),
            new (FilterType.Equal, null, TimerField.Ignore, expected.Ignore),
        ];

        // Act

        var query = new TimerFieldQuery(filters: [new TimerFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        TimerHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var models = Enumerable.Range(0, 20).Select(_ => TimerHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, models[0].Id),
            new (FilterType.Equal, null, TimerField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, models[2].RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, models[3].Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, models[4].NextExecutionDateTime),
            // new (FilterType.Equal, null, TimerField.Ignore, models[5].Ignore),
        ];

        // Act

        var query = new TimerFieldQuery(filters: [new TimerFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            TimerHelper.AssertModels(expectedModel, model, true);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var models = Enumerable.Range(0, 20).Select(_ => TimerHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, models[0].Id),
            new (FilterType.Equal, null, TimerField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, models[2].RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, models[3].Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, models[4].NextExecutionDateTime),
            // new (FilterType.Equal, null, TimerField.Ignore, models[5].Ignore),
        ];

        var notFilters = filters.Select(f => new TimerFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new TimerFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

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
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        var filter = new TimerFieldFilter(FilterType.Equal, null, TimerField.ProcessId, processId);
        var sort = new TimerFieldSort(TimerField.Name, Direction.Asc);

        // Act

        var queryPage1 = new TimerFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchProcessesTimersAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new TimerFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchProcessesTimersAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new TimerFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchProcessesTimersAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

        TimerModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                TimerHelper.AssertModels(expected, model, true);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = await TimerHelper.ExclusiveSearchPermissionsApi(service);

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new TimerFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesTimersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            TimerHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await TimerHelper.NoPermissionsApi(service);

        var query = new TimerFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchProcessesTimersAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}