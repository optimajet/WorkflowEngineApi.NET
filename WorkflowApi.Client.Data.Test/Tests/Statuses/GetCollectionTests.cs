using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Statuses;

[TestClass]
public class GetCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataStatusesGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            StatusHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataStatusesGetCollectionAsync(search: models.First().RuntimeId);

        TestLogger.LogApiCalled(new { search = models.First().RuntimeId }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        StatusHelper.AssertModels(models.First(), actual);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<StatusFieldFilter> filters = [
            // new (FilterType.Equal, null, StatusField.Id, expected.Id),
            new (FilterType.Equal, null, StatusField.Status, expected.StatusCode),
            new (FilterType.Equal, null, StatusField.Lock, expected.VarLock),
            new (FilterType.Equal, null, StatusField.RuntimeId, expected.RuntimeId),
            new (FilterType.Equal, null, StatusField.SetTime, expected.SetTime),
        ];

        // Act

        var result = await api.WorkflowApiDataStatusesGetCollectionAsync(filters: filters);

        TestLogger.LogApiCalled(new { filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        StatusHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<StatusFieldFilter> filters = [
            // new (FilterType.Equal, null, StatusField.Id, expected.Id),
            new (FilterType.Equal, null, StatusField.Status, expected.StatusCode),
            new (FilterType.Equal, null, StatusField.Lock, expected.VarLock),
            new (FilterType.Equal, null, StatusField.RuntimeId, expected.RuntimeId),
            new (FilterType.Equal, null, StatusField.SetTime, expected.SetTime),
        ];

        List<StatusFieldFilter> andFilter = [new (FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataStatusesGetCollectionAsync(filters: andFilter);

        TestLogger.LogApiCalled(new { filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        StatusHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(20);

        await repository.CreateAsync(models);

        List<StatusFieldFilter> filters = [
            // new (FilterType.Equal, null, StatusField.Id, models[0].Id),
            // new (FilterType.Equal, null, StatusField.Status, models[1].StatusCode),
            new (FilterType.Equal, null, StatusField.Lock, models[0].VarLock),
            new (FilterType.Equal, null, StatusField.RuntimeId, models[1].RuntimeId),
            new (FilterType.Equal, null, StatusField.SetTime, models[2].SetTime),
        ];

        List<StatusFieldFilter> orFilter = [new StatusFieldFilter(FilterType.Or, filters)];

        // Act

        var result = await api.WorkflowApiDataStatusesGetCollectionAsync(filters: orFilter);

        TestLogger.LogApiCalled(new { filters }, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            StatusHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(20);

        await repository.CreateAsync(models);

        List<StatusFieldFilter> filters = [
            // new (FilterType.Equal, null, StatusField.Id, models[0].Id),
            // new (FilterType.Equal, null, StatusField.Status, models[1].StatusCode),
            new (FilterType.Equal, null, StatusField.Lock, models[0].VarLock),
            new (FilterType.Equal, null, StatusField.RuntimeId, models[1].RuntimeId),
            new (FilterType.Equal, null, StatusField.SetTime, models[2].SetTime),
        ];

        var notFilters = filters.Select(f => new StatusFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var result = await api.WorkflowApiDataStatusesGetCollectionAsync(filters: notFilters);

        TestLogger.LogApiCalled(new { filters }, result);

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
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(30);

        await repository.CreateAsync(models);

        List<StatusFieldFilter> filters = [new StatusFieldFilter(FilterType.In, null, StatusField.RuntimeId, models.Select(m => m.RuntimeId))];
        List<StatusFieldSort> sorts = [new StatusFieldSort(StatusField.RuntimeId, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataStatusesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 0 }, page1);

        var page2 = await api.WorkflowApiDataStatusesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 10 }, page2);

        var page3 = await api.WorkflowApiDataStatusesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 20 }, page3);

        StatusModelGetCollectionResponse[] pages = [page1, page2, page3];

        // Assert

        var idsCount = pages.SelectMany(p => p.Collection).Select(c => c.RuntimeId).Distinct().Count();

        Assert.AreEqual(30, idsCount);

        foreach (var page in pages)
        {
            Assert.AreEqual(30, page.Total);
            Assert.AreEqual(10, page.Collection.Count);

            foreach (var model in page.Collection)
            {
                var expected = models.First(x => x.RuntimeId == model.RuntimeId);
                StatusHelper.AssertModels(expected, model);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = await StatusHelper.ExclusivePermissionsApi(service, "get-collection");

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataStatusesGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            StatusHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await StatusHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataStatusesGetCollectionAsync()
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}