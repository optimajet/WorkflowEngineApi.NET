using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Transitions;

[TestClass]
public class DeleteCollectionTests
{
    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = TransitionHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesTransitionsDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, deleteResult);

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
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<TransitionFieldFilter> filters = [
            new (FilterType.Equal, null, TransitionField.Id, expected.Id),
            // new (FilterType.Equal, null, TransitionField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, expected.ExecutorIdentityId),
            new (FilterType.Equal, null, TransitionField.ActorIdentityId, expected.ActorIdentityId),
            new (FilterType.Equal, null, TransitionField.ExecutorName, expected.ExecutorName),
            new (FilterType.Equal, null, TransitionField.ActorName, expected.ActorName),
            new (FilterType.Equal, null, TransitionField.FromActivityName, expected.FromActivityName),
            new (FilterType.Equal, null, TransitionField.ToActivityName, expected.ToActivityName),
            new (FilterType.Equal, null, TransitionField.ToStateName, expected.ToStateName),
            new (FilterType.Equal, null, TransitionField.TransitionTime, expected.TransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionClassifier, expected.TransitionClassifier),
            new (FilterType.Equal, null, TransitionField.IsFinalised, expected.IsFinalised),
            new (FilterType.Equal, null, TransitionField.FromStateName, expected.FromStateName),
            new (FilterType.Equal, null, TransitionField.TriggerName, expected.TriggerName),
            new (FilterType.Equal, null, TransitionField.StartTransitionTime, expected.StartTransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionDuration, expected.TransitionDuration),
        ];

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesTransitionsDeleteCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new { processId, filters }, deleteResult);

        // Assert

        Assert.AreEqual(1, deleteResult.DeletedCount);
        Assert.IsNull(await repository.GetAsync(expected.Id));

        foreach (var model in models.Where(m => m.Id != expected.Id))
        {
            var notDeletedModel = await repository.GetAsync(model.Id);
            TransitionHelper.AssertModels(model, notDeletedModel, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<TransitionFieldFilter> filters = [
            new (FilterType.Equal, null, TransitionField.Id, models[0].Id),
            // new (FilterType.Equal, null, TransitionField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, models[1].ExecutorIdentityId),
            new (FilterType.Equal, null, TransitionField.ActorIdentityId, models[2].ActorIdentityId),
            new (FilterType.Equal, null, TransitionField.ExecutorName, models[3].ExecutorName),
            new (FilterType.Equal, null, TransitionField.ActorName, models[4].ActorName),
            new (FilterType.Equal, null, TransitionField.FromActivityName, models[5].FromActivityName),
            new (FilterType.Equal, null, TransitionField.ToActivityName, models[6].ToActivityName),
            new (FilterType.Equal, null, TransitionField.ToStateName, models[7].ToStateName),
            new (FilterType.Equal, null, TransitionField.TransitionTime, models[8].TransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionClassifier, models[9].TransitionClassifier),
            // new (FilterType.Equal, null, TransitionField.IsFinalised, models[11].IsFinalised),
            new (FilterType.Equal, null, TransitionField.FromStateName, models[10].FromStateName),
            new (FilterType.Equal, null, TransitionField.TriggerName, models[11].TriggerName),
            new (FilterType.Equal, null, TransitionField.StartTransitionTime, models[12].StartTransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionDuration, models[13].TransitionDuration),
        ];

        List<TransitionFieldFilter> orFilters = [new TransitionFieldFilter(FilterType.Or, filters)];

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesTransitionsDeleteCollectionAsync(processId, filters: orFilters);

        TestLogger.LogApiCalled(new { processId, filters = orFilters }, deleteResult);

        // Assert

        Assert.AreEqual(filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            Assert.IsNull(await repository.GetAsync(models[i].Id));
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            TransitionHelper.AssertModels(models[i], notDeletedModel, true);
        }
    }

    [ClientTest(HostId.DataHost)] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<TransitionFieldFilter> filters = [
            new (FilterType.Equal, null, TransitionField.Id, models[0].Id),
            // new (FilterType.Equal, null, TransitionField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, TransitionField.ExecutorIdentityId, models[1].ExecutorIdentityId),
            new (FilterType.Equal, null, TransitionField.ActorIdentityId, models[2].ActorIdentityId),
            new (FilterType.Equal, null, TransitionField.ExecutorName, models[3].ExecutorName),
            new (FilterType.Equal, null, TransitionField.ActorName, models[4].ActorName),
            new (FilterType.Equal, null, TransitionField.FromActivityName, models[5].FromActivityName),
            new (FilterType.Equal, null, TransitionField.ToActivityName, models[6].ToActivityName),
            new (FilterType.Equal, null, TransitionField.ToStateName, models[7].ToStateName),
            new (FilterType.Equal, null, TransitionField.TransitionTime, models[8].TransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionClassifier, models[9].TransitionClassifier),
            // new (FilterType.Equal, null, TransitionField.IsFinalised, models[11].IsFinalised),
            new (FilterType.Equal, null, TransitionField.FromStateName, models[10].FromStateName),
            new (FilterType.Equal, null, TransitionField.TriggerName, models[11].TriggerName),
            new (FilterType.Equal, null, TransitionField.StartTransitionTime, models[12].StartTransitionTime),
            new (FilterType.Equal, null, TransitionField.TransitionDuration, models[13].TransitionDuration),
        ];

        var notFilters = filters.Select(f => new TransitionFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesTransitionsDeleteCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new { processId, filters = notFilters }, deleteResult);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            TransitionHelper.AssertModels(models[i], notDeletedModel, true);
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
        var repository = service.Repository.Transitions;
        var api = await TransitionHelper.ExclusivePermissionsApi(service, "delete-collection");

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = TransitionHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesTransitionsDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, deleteResult);

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

        var api = await TransitionHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesTransitionsDeleteCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}