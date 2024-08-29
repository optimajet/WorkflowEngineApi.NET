using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimaJet.DataEngine;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Parameters;

[TestClass]
public class SearchTests
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

        var query = new ParameterFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesParametersAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.ProcessId == models[i].ProcessId && x.Name == models[i].Name);
            ParameterHelper.AssertModels(models[i], actual, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
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

        ParameterHelper.AssertModels(models.First(), actual, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
    }

    [ClientTest(ProviderName.Mongo)] // Mongo does not support parameter id field
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

    [ClientTest]
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

    [ClientTest]
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

    [ClientTest]
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

    [ClientTest]
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
            ParameterHelper.AssertModels(models[i], actual, service.Configuration.AppConfiguration.Provider != Provider.Mongo);
        }
    }

    [ClientTest]
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