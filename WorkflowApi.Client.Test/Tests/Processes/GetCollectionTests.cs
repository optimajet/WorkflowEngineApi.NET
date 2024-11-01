using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Processes;

[TestClass]
public class GetCollectionTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataProcessesGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            ProcessHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataProcessesGetCollectionAsync(search: models.First().TenantId);

        TestLogger.LogApiCalled(new { search = models.First().TenantId }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        ProcessHelper.AssertModels(models.First(), actual);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, expected.Id),
            new (FilterType.Equal, null, ProcessField.StateName, expected.StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, expected.ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, expected.SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, expected.PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, expected.PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, expected.PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, expected.PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, expected.PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, expected.PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, expected.ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, expected.IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, expected.TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, expected.StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, expected.SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, expected.CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, expected.LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, expected.CalendarName),
        ];

        // Act

        var result = await api.WorkflowApiDataProcessesGetCollectionAsync(filters: filters);

        TestLogger.LogApiCalled(new { filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ProcessHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, expected.Id),
            new (FilterType.Equal, null, ProcessField.StateName, expected.StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, expected.ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, expected.SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, expected.PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, expected.PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, expected.PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, expected.PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, expected.PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, expected.PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, expected.ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, expected.RootProcessId),
            new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, expected.IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, expected.TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, expected.StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, expected.SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, expected.CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, expected.LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, expected.CalendarName),
        ];

        List<ProcessFieldFilter> andFilters = [new ProcessFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesGetCollectionAsync(filters: andFilters);

        TestLogger.LogApiCalled(new { filters = andFilters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ProcessHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, models[0].Id),
            new (FilterType.Equal, null, ProcessField.StateName, models[1].StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, models[2].ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, models[3].SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, models[4].PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, models[5].PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, models[6].PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, models[7].PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, models[8].PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, models[9].PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, models[10].ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, models[11].RootProcessId),
            // new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, models[12].IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, models[12].TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, models[13].StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, models[14].SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, models[15].CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, models[16].LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, models[17].CalendarName),
        ];

        List<ProcessFieldFilter> orFilters = [new ProcessFieldFilter(FilterType.Or, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesGetCollectionAsync(filters: orFilters);

        TestLogger.LogApiCalled(new { filters = orFilters }, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            ProcessHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(20);

        await repository.CreateAsync(models);

        List<ProcessFieldFilter> filters = [
            new (FilterType.Equal, null, ProcessField.Id, models[0].Id),
            new (FilterType.Equal, null, ProcessField.StateName, models[1].StateName),
            new (FilterType.Equal, null, ProcessField.ActivityName, models[2].ActivityName),
            new (FilterType.Equal, null, ProcessField.SchemeId, models[3].SchemeId),
            new (FilterType.Equal, null, ProcessField.PreviousState, models[4].PreviousState),
            new (FilterType.Equal, null, ProcessField.PreviousStateForDirect, models[5].PreviousStateForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousStateForReverse, models[6].PreviousStateForReverse),
            new (FilterType.Equal, null, ProcessField.PreviousActivity, models[7].PreviousActivity),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForDirect, models[8].PreviousActivityForDirect),
            new (FilterType.Equal, null, ProcessField.PreviousActivityForReverse, models[9].PreviousActivityForReverse),
            new (FilterType.Equal, null, ProcessField.ParentProcessId, models[10].ParentProcessId),
            new (FilterType.Equal, null, ProcessField.RootProcessId, models[11].RootProcessId),
            // new (FilterType.Equal, null, ProcessField.IsDeterminingParametersChanged, models[12].IsDeterminingParametersChanged),
            new (FilterType.Equal, null, ProcessField.TenantId, models[12].TenantId),
            new (FilterType.Equal, null, ProcessField.StartingTransition, models[13].StartingTransition),
            new (FilterType.Equal, null, ProcessField.SubprocessName, models[14].SubprocessName),
            new (FilterType.Equal, null, ProcessField.CreationDate, models[15].CreationDate),
            new (FilterType.Equal, null, ProcessField.LastTransitionDate, models[16].LastTransitionDate),
            new (FilterType.Equal, null, ProcessField.CalendarName, models[17].CalendarName),
        ];

        var notFilters = filters.Select(f => new ProcessFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var result = await api.WorkflowApiDataProcessesGetCollectionAsync(filters: notFilters);

        TestLogger.LogApiCalled(new { filters = notFilters }, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            Assert.IsNull(model);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = service.Client.Processes;

        var models = ProcessHelper.Generate(30);

        await repository.CreateAsync(models);

        List<ProcessFieldFilter> filters = [new ProcessFieldFilter(FilterType.In, null, ProcessField.Id, models.Select(m => m.Id))];
        List<ProcessFieldSort> sorts = [new ProcessFieldSort(ProcessField.Id, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataProcessesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 0 }, page1);

        var page2 = await api.WorkflowApiDataProcessesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 10 }, page2);

        var page3 = await api.WorkflowApiDataProcessesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 20 }, page3);

        ProcessModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                ProcessHelper.AssertModels(expected, model);
            }
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Processes;
        var api = await ProcessHelper.ExclusivePermissionsApi(service, "get-collection");

        var models = ProcessHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataProcessesGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            ProcessHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ProcessHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesGetCollectionAsync()
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}