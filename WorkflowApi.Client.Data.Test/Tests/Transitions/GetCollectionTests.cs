using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.Transitions;

[TestClass]
public class GetCollectionTests
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
        await repository.CreateAsync(TransitionHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            TransitionHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, search: models.First().TriggerName);

        TestLogger.LogApiCalled(new { processId, models.First().TriggerName }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        TransitionHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
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

        var result = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new { processId, filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        TransitionHelper.AssertModels(expected, result.Collection.First(), true);
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

        List<TransitionFieldFilter> andFilters = [new TransitionFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, filters: andFilters);

        TestLogger.LogApiCalled(new { processId, filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        TransitionHelper.AssertModels(expected, result.Collection.First(), true);
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

        var result = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, filters: orFilters);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            TransitionHelper.AssertModels(expectedModel, model, true);
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

        var result = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new { processId, filters = notFilters }, result);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, result.Total);

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
        var repository = service.Repository.Transitions;
        var api = service.Client.Transitions;

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        List<TransitionFieldSort> sorts = [new TransitionFieldSort(TransitionField.TriggerName, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new { processId, sorts, take = 10, skip = 0 }, page1);

        var page2 = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new { processId, sorts, take = 10, skip = 10 }, page2);

        var page3 = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new { processId, sorts, take = 10, skip = 20 }, page3);

        TransitionModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                TransitionHelper.AssertModels(expected, model, true);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Transitions;
        var api = await TransitionHelper.ExclusivePermissionsApi(service, "get-collection");

        var processId = Guid.NewGuid();
        var models = TransitionHelper.Generate(processId, 3);

        await repository.CreateAsync(models);
        await repository.CreateAsync(TransitionHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            TransitionHelper.AssertModels(model, actual, true);
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
            async () => await api.WorkflowApiDataProcessesTransitionsGetCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}