using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Runtimes;

[TestClass]
public class GetCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataRuntimesGetCollectionAsync();

        TestLogger.LogApiCalled(new { }, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            RuntimeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataRuntimesGetCollectionAsync(search: models.First().Id);

        TestLogger.LogApiCalled(new { search = models.First().Id }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        RuntimeHelper.AssertModels(models.First(), actual);
    }

    [ClientTest(HostId.DataHost)]
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
            // new (FilterType.Equal, null, RuntimeField.Status, expected.Status),
            new (FilterType.Equal, null, RuntimeField.RestorerId, expected.RestorerId),
            new (FilterType.Equal, null, RuntimeField.NextTimerTime, expected.NextTimerTime),
            new (FilterType.Equal, null, RuntimeField.NextServiceTimerTime, expected.NextServiceTimerTime),
            new (FilterType.Equal, null, RuntimeField.LastAliveSignal, expected.LastAliveSignal),
        ];

        // Act

        var result = await api.WorkflowApiDataRuntimesGetCollectionAsync(filters: filters);

        TestLogger.LogApiCalled(new { filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        RuntimeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
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
            // new (FilterType.Equal, null, RuntimeField.Status, expected.Status),
            new (FilterType.Equal, null, RuntimeField.RestorerId, expected.RestorerId),
            new (FilterType.Equal, null, RuntimeField.NextTimerTime, expected.NextTimerTime),
            new (FilterType.Equal, null, RuntimeField.NextServiceTimerTime, expected.NextServiceTimerTime),
            new (FilterType.Equal, null, RuntimeField.LastAliveSignal, expected.LastAliveSignal),
        ];

        List<RuntimeFieldFilter> andFilter = [new RuntimeFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataRuntimesGetCollectionAsync(filters: andFilter);

        TestLogger.LogApiCalled(new { filters = andFilter }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        RuntimeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
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

        List<RuntimeFieldFilter> orFilter = [new RuntimeFieldFilter(FilterType.Or, filters)];

        // Act

        var result = await api.WorkflowApiDataRuntimesGetCollectionAsync(filters: orFilter);

        TestLogger.LogApiCalled(new { filters = orFilter }, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            RuntimeHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest(HostId.DataHost)] 
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

        var result = await api.WorkflowApiDataRuntimesGetCollectionAsync(filters: notFilters);

        TestLogger.LogApiCalled(new { filters = notFilters }, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            Assert.IsNull(model);
        }
    }

    [ClientTest(HostId.DataHost, ExcludeProviders = [ProviderName.Mongo, ProviderName.Mssql, ProviderName.Mysql, ProviderName.Oracle, ProviderName.Sqlite])]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = service.Client.Runtimes;

        var models = RuntimeHelper.Generate(30);

        await repository.CreateAsync(models);

        List<RuntimeFieldFilter> filters = [new RuntimeFieldFilter(FilterType.In, null, RuntimeField.RuntimeId, models.Select(m => m.Id))];
        List<RuntimeFieldSort> sorts = [new RuntimeFieldSort(RuntimeField.RuntimeId, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataRuntimesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 0 }, page1);

        var page2 = await api.WorkflowApiDataRuntimesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 10 }, page2);

        var page3 = await api.WorkflowApiDataRuntimesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 20 }, page3);

        RuntimeModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                RuntimeHelper.AssertModels(expected, model);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Runtimes;
        var api = await RuntimeHelper.ExclusivePermissionsApi(service, "get-collection");

        var models = RuntimeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataRuntimesGetCollectionAsync();

        TestLogger.LogApiCalled(new { }, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            RuntimeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await RuntimeHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataRuntimesGetCollectionAsync()
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}