using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Timers;

[TestClass]
public class GetCollectionTests
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
        await repository.CreateAsync(TimerHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            TimerHelper.AssertModels(model, actual, true);
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

        // Act

        var result = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, search: models.First().Name);

        TestLogger.LogApiCalled(new {processId, search = models.First().Name}, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        TimerHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, expected.Id),
            // new (FilterType.Equal, null, TimerField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, expected.Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, expected.NextExecutionDateTime),
            new (FilterType.Equal, null, TimerField.Ignore, expected.Ignore),
        ];

        // Act

        var result = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new {processId, filters}, result);

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

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, expected.Id),
            // new (FilterType.Equal, null, TimerField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, expected.Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, expected.NextExecutionDateTime),
            new (FilterType.Equal, null, TimerField.Ignore, expected.Ignore),
        ];

        List<TimerFieldFilter> andFilters = [new TimerFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, filters: andFilters);

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

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, models[0].Id),
            // new (FilterType.Equal, null, TimerField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, models[1].RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, models[2].Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, models[3].NextExecutionDateTime),
            // new (FilterType.Equal, null, TimerField.Ignore, models[5].Ignore),
        ];

        List<TimerFieldFilter> orFilters = [new TimerFieldFilter(FilterType.Or, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, filters: orFilters);

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

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<TimerFieldFilter> filters = [
            new (FilterType.Equal, null, TimerField.Id, models[0].Id),
            // new (FilterType.Equal, null, TimerField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TimerField.RootProcessId, models[1].RootProcessId),
            new (FilterType.Equal, null, TimerField.Name, models[2].Name),
            new (FilterType.Equal, null, TimerField.NextExecutionDateTime, models[3].NextExecutionDateTime),
            // new (FilterType.Equal, null, TimerField.Ignore, models[5].Ignore),
        ];

        var notFilters = filters.Select(f => new TimerFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var result = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, filters: notFilters);

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
        var repository = service.Repository.Timers;
        var api = service.Client.Timers;

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        List<TimerFieldSort> sorts = [new TimerFieldSort(TimerField.Name, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 0}, page1);

        var page2 = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 10}, page2);

        var page3 = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 20}, page3);

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
        var api = await TimerHelper.ExclusivePermissionsApi(service, "get-collection");

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);
        await repository.CreateAsync(TimerHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesTimersGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            TimerHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await TimerHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTimersGetCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}