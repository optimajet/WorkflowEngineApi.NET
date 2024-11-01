using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Parameters;

[TestClass]
public class GetCollectionTests
{
    [ClientTest]
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
            ParameterHelper.AssertModels(model, actual, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
        }
    }

    [ClientTest]
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

        ParameterHelper.AssertModels(models.First(), actual, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
    }

    [ClientTest]
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
        ParameterHelper.AssertModels(expected, result.Collection.First(), service.Configuration.AppConfiguration.Provider != Provider.Mongo);
    }

    [ClientTest]
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
        ParameterHelper.AssertModels(expected, result.Collection.First(), service.Configuration.AppConfiguration.Provider != Provider.Mongo);
    }

    [ClientTest] 
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
            ParameterHelper.AssertModels(expectedModel, model, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
        }
    }

    [ClientTest] 
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

    [ClientTest] 
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
                ParameterHelper.AssertModels(expected, model, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
            }
        }
    }

    [ClientTest]
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
            ParameterHelper.AssertModels(model, actual, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
        }
    }

    [ClientTest]
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