using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Schemes;

[TestClass]
public class DeleteCollectionTests
{
    [ClientTest]
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

        var deleteResult = await api.WorkflowApiDataSchemesDeleteCollectionAsync();

        TestLogger.LogApiCalled(new {}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount >= models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            Assert.IsNull(result);
        }
    }

    [ClientTest]
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

        // Act

        var deleteResult = await api.WorkflowApiDataSchemesDeleteCollectionAsync(filters: filters);

        TestLogger.LogApiCalled(new { filters }, deleteResult);

        // Assert

        Assert.AreEqual(1, deleteResult.DeletedCount);
        Assert.IsNull(await repository.GetAsync(expected.Code));

        foreach (var model in models.Where(m => m.Code != expected.Code))
        {
            var notDeletedModel = await repository.GetAsync(model.Code);
            SchemeHelper.AssertModels(model, notDeletedModel);
        }
    }

    [ClientTest]
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

        var deleteResult = await api.WorkflowApiDataSchemesDeleteCollectionAsync(filters: orFilters);

        TestLogger.LogApiCalled(new { filters }, deleteResult);

        // Assert

        Assert.AreEqual(filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            Assert.IsNull(await repository.GetAsync(models[i].Code));
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Code);
            SchemeHelper.AssertModels(models[i], notDeletedModel);
        }
    }

    [ClientTest] 
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
        var inFilter = new SchemeFieldFilter(FilterType.In, null, SchemeField.Code, models.Select(x => x.Code).ToList());
        List<SchemeFieldFilter> finalFilters = [..notFilters, inFilter];

        // Act

        var deleteResult = await api.WorkflowApiDataSchemesDeleteCollectionAsync(filters: finalFilters);

        TestLogger.LogApiCalled(new { filters }, deleteResult);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Code);
            SchemeHelper.AssertModels(models[i], notDeletedModel);
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            Assert.IsNull(await repository.GetAsync(models[i].Code));
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Schemes;
        var api = await SchemeHelper.ExclusivePermissionsApi(service, "delete-collection");

        var models = SchemeHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var deleteResult = await api.WorkflowApiDataSchemesDeleteCollectionAsync();

        TestLogger.LogApiCalled(new {}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount >= models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Code);
            Assert.IsNull(result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await SchemeHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataSchemesDeleteCollectionAsync()
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}