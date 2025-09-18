using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Timers;

[TestClass]
public class DeleteCollectionTests
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

        var extraModel = TimerHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesTimersDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount == models.Length);

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

        var deleteResult = await api.WorkflowApiDataProcessesTimersDeleteCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new {processId, filters}, deleteResult);

        // Assert

        Assert.AreEqual(1, deleteResult.DeletedCount);
        Assert.IsNull(await repository.GetAsync(expected.Id));

        foreach (var model in models.Where(m => m.Id != expected.Id))
        {
            var notDeletedModel = await repository.GetAsync(model.Id);
            TimerHelper.AssertModels(model, notDeletedModel, true);
        }
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

        var deleteResult = await api.WorkflowApiDataProcessesTimersDeleteCollectionAsync(processId, filters: orFilters);

        TestLogger.LogApiCalled(new {processId, filters = orFilters}, deleteResult);

        // Assert

        Assert.AreEqual(filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            Assert.IsNull(await repository.GetAsync(models[i].Id));
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            TimerHelper.AssertModels(models[i], notDeletedModel, true);
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

        var deleteResult = await api.WorkflowApiDataProcessesTimersDeleteCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new {processId, filters = notFilters}, deleteResult);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            TimerHelper.AssertModels(models[i], notDeletedModel, true);
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
        var repository = service.Repository.Timers;
        var api = await TimerHelper.ExclusivePermissionsApi(service, "delete-collection");

        var processId = Guid.NewGuid();
        var models = TimerHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = TimerHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesTimersDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount == models.Length);

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

        var api = await TimerHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTimersDeleteCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}