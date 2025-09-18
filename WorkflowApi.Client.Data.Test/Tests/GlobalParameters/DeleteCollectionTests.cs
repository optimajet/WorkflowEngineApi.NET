using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.GlobalParameters;

[TestClass]
public class DeleteCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var deleteResult = await api.WorkflowApiDataGlobalParametersDeleteCollectionAsync();

        TestLogger.LogApiCalled(new {}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount >= models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<GlobalParameterFieldFilter> filters = [
            new (FilterType.Equal, null, GlobalParameterField.Id, expected.Id),
            new (FilterType.Equal, null, GlobalParameterField.Type, expected.Type),
            new (FilterType.Equal, null, GlobalParameterField.Name, expected.Name),
            new (FilterType.Equal, null, GlobalParameterField.Value, expected.Value),
        ];

        // Act

        var deleteResult = await api.WorkflowApiDataGlobalParametersDeleteCollectionAsync(filters: filters);

        TestLogger.LogApiCalled(new {filters}, deleteResult);

        // Assert

        Assert.AreEqual(1, deleteResult.DeletedCount);

        foreach (var model in models.Where(m => m.Id != expected.Id))
        {
            Assert.IsNull(await repository.GetAsync(expected.Id));
            var notDeletedModel = await repository.GetAsync(model.Id);
            GlobalParameterHelper.AssertModels(model, notDeletedModel, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(20);

        await repository.CreateAsync(models);

        List<GlobalParameterFieldFilter> filters = [
            new (FilterType.Equal, null, GlobalParameterField.Id, models[0].Id),
            new (FilterType.Equal, null, GlobalParameterField.Type, models[1].Type),
            new (FilterType.Equal, null, GlobalParameterField.Name, models[2].Name),
            new (FilterType.Equal, null, GlobalParameterField.Value, models[3].Value),
        ];

        List<GlobalParameterFieldFilter> orFilters = [new GlobalParameterFieldFilter(FilterType.Or, filters)];

        // Act

        var deleteResult = await api.WorkflowApiDataGlobalParametersDeleteCollectionAsync(filters: orFilters);

        TestLogger.LogApiCalled(new {filters = orFilters}, deleteResult);

        // Assert

        Assert.AreEqual(filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            Assert.IsNull(await repository.GetAsync(models[i].Id));
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            GlobalParameterHelper.AssertModels(models[i], notDeletedModel, true);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = service.Client.GlobalParameters;

        var models = GlobalParameterHelper.Generate(20);

        await repository.CreateAsync(models);

        List<GlobalParameterFieldFilter> filters = [
            new (FilterType.Equal, null, GlobalParameterField.Id, models[0].Id),
            new (FilterType.Equal, null, GlobalParameterField.Type, models[1].Type),
            new (FilterType.Equal, null, GlobalParameterField.Name, models[2].Name),
            new (FilterType.Equal, null, GlobalParameterField.Value, models[3].Value),
        ];

        var notFilters = filters.Select(f => new GlobalParameterFieldFilter(FilterType.Not, [f])).ToList();
        var inFilter = new GlobalParameterFieldFilter(FilterType.In, null, GlobalParameterField.Id, models.Select(x => x.Id).ToList());

        List<GlobalParameterFieldFilter> finalFilters = [..notFilters, inFilter];

        // Act

        var deleteResult = await api.WorkflowApiDataGlobalParametersDeleteCollectionAsync(filters: finalFilters);

        TestLogger.LogApiCalled(new {filters = finalFilters}, deleteResult);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            GlobalParameterHelper.AssertModels(models[i], notDeletedModel, true);
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            Assert.IsNull(await repository.GetAsync(models[i].Id));
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.GlobalParameters;
        var api = await GlobalParameterHelper.ExclusivePermissionsApi(service, "delete-collection");

        var models = GlobalParameterHelper.Generate(3);

        await repository.CreateAsync(models);

        // Act

        var deleteResult = await api.WorkflowApiDataGlobalParametersDeleteCollectionAsync();

        TestLogger.LogApiCalled(new {}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount >= models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await GlobalParameterHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataGlobalParametersDeleteCollectionAsync()
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}