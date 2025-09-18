using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Schemes;

[TestClass]
public class GetCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataSchemesGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Code == models[i].Code);
            SchemeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataSchemesGetCollectionAsync(search: models.First().Code);

        TestLogger.LogApiCalled(new { search = models.First().Code }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        SchemeHelper.AssertModels(models.First(), actual);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, expected.Code),
            // new (FilterType.Equal, null, SchemeField.Scheme, expected.Scheme),
            new (FilterType.Equal, null, SchemeField.CanBeInlined, expected.CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, expected.InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, expected.Tags),
        ];

        // Act

        var result = await api.WorkflowApiDataSchemesGetCollectionAsync(filters: filters);

        TestLogger.LogApiCalled(new { filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        SchemeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, expected.Code),
            // new (FilterType.Equal, null, SchemeField.Scheme, expected.Scheme),
            new (FilterType.Equal, null, SchemeField.CanBeInlined, expected.CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, expected.InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, expected.Tags),
        ];

        List<SchemeFieldFilter> andFilters = [new SchemeFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataSchemesGetCollectionAsync(filters: andFilters);

        TestLogger.LogApiCalled(new { filters = andFilters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        SchemeHelper.AssertModels(expected, result.Collection.First());
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, models[0].Code),
            // new (FilterType.Equal, null, SchemeField.Scheme, models[1].Scheme),
            // new (FilterType.Equal, null, SchemeField.CanBeInlined, models[2].CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, models[1].InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, models[3].Tags),
        ];

        List<SchemeFieldFilter> orFilters = [new SchemeFieldFilter(FilterType.Or, filters)];

        // Act

        var result = await api.WorkflowApiDataSchemesGetCollectionAsync(filters: orFilters);

        TestLogger.LogApiCalled(new { filters = orFilters }, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Code == model.Code);
            SchemeHelper.AssertModels(expectedModel, model);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(20);

        await repository.CreateAsync(models);

        List<SchemeFieldFilter> filters = [
            new (FilterType.Equal, null, SchemeField.Code, models[0].Code),
            // new (FilterType.Equal, null, SchemeField.Scheme, models[1].Scheme),
            // new (FilterType.Equal, null, SchemeField.CanBeInlined, models[2].CanBeInlined),
            // new (FilterType.Equal, null, SchemeField.InlinedSchemes, models[1].InlinedSchemes),
            // new (FilterType.Equal, null, SchemeField.Tags, models[3].Tags),
        ];

        var notFilters = filters.Select(f => new SchemeFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var result = await api.WorkflowApiDataSchemesGetCollectionAsync(filters: notFilters);

        TestLogger.LogApiCalled(new { filters = notFilters }, result);

        // Assert

        for (int i = 0; i < filters.Count; i++)
        {
            var model = result.Collection.FirstOrDefault(x => x.Code == models[i].Code);
            Assert.IsNull(model);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithPagingTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = service.Client.Schemes;

        var models = SchemeHelper.Generate(30);

        await repository.CreateAsync(models);

        List<SchemeFieldFilter> filters = [new SchemeFieldFilter(FilterType.In, null, SchemeField.Code, models.Select(m => m.Code))];
        List<SchemeFieldSort> sorts = [new SchemeFieldSort(SchemeField.Code, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataSchemesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 0 }, page1);

        var page2 = await api.WorkflowApiDataSchemesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 10 }, page2);

        var page3 = await api.WorkflowApiDataSchemesGetCollectionAsync(filters: filters, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new { filters, sorts, take = 10, skip = 20 }, page3);

        SchemeModelGetCollectionResponse[] pages = [page1, page2, page3];

        // Assert

        var idsCount = pages.SelectMany(p => p.Collection).Select(c => c.Code).Distinct().Count();

        Assert.AreEqual(30, idsCount);

        foreach (var page in pages)
        {
            Assert.AreEqual(30, page.Total);
            Assert.AreEqual(10, page.Collection.Count);

            foreach (var model in page.Collection)
            {
                var expected = models.First(x => x.Code == model.Code);
                SchemeHelper.AssertModels(expected, model);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusivePermissionsApi(service, "get-collection");

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataSchemesGetCollectionAsync();

        TestLogger.LogApiCalled(new {}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Code == models[i].Code);
            SchemeHelper.AssertModels(models[i], actual);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await SchemeHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesGetCollectionAsync()
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}