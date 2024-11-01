using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Statuses;

[TestClass]
public class SearchTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new StatusFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            StatusHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new StatusFieldQuery(
            search: models.First().RuntimeId
        );

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        StatusHelper.AssertModels(models.First(), actual);
    }

    [ClientTest(ProviderName.Mongo)] // Mongo does not support status id field
    [TestMethod]
    public async Task ExecuteWithIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        var query = new StatusFieldQuery
        {
            Filters = [new (FilterType.Equal, null, StatusField.Id, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

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
    public async Task ExecuteWithStatusEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().StatusCode;

        var query = new StatusFieldQuery
        {
            Filters = [new (FilterType.Equal, null, StatusField.Status, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.StatusCode);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithLockEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().VarLock;

        var query = new StatusFieldQuery
        {
            Filters = [new (FilterType.Equal, null, StatusField.Lock, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

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
    public async Task ExecuteWithRuntimeIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().RuntimeId;

        var query = new StatusFieldQuery
        {
            Filters = [new (FilterType.Equal, null, StatusField.RuntimeId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.RuntimeId);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSetTimeEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var expected = models.First().SetTime;

        var query = new StatusFieldQuery
        {
            Filters = [new (FilterType.Equal, null, StatusField.SetTime, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.SetTime);
        }
    }

    [ClientTest]
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

        var query = new StatusFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        StatusHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
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

        // Act

        var query = new StatusFieldQuery(filters: [new StatusFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        StatusHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest]
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

        // Act

        var query = new StatusFieldQuery(filters: [new StatusFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            StatusHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest] 
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

        var query = new StatusFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

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
        var repository = service.Repository.Statuses;
        var api = service.Client.Statuses;

        var models = StatusHelper.Generate(30);

        await repository.CreateAsync(models);

        var filter = new StatusFieldFilter(FilterType.In, null, StatusField.RuntimeId, models.Select(m => m.RuntimeId));
        var sort = new StatusFieldSort(StatusField.RuntimeId, Direction.Asc);

        // Act

        var queryPage1 = new StatusFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchStatusesAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new StatusFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchStatusesAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new StatusFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchStatusesAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

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

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Statuses;
        var api = await StatusHelper.ExclusiveSearchPermissionsApi(service);

        var models = StatusHelper.Generate(3);

        await repository.CreateAsync(models);

        var query = new StatusFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchStatusesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            StatusHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await StatusHelper.NoPermissionsApi(service);

        var query = new StatusFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchStatusesAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}