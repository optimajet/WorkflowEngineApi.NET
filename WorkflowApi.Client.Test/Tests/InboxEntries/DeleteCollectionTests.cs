using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.InboxEntries;

[TestClass]
public class DeleteCollectionTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = InboxEntryHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesInboxEntriesDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount == models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<InboxEntryFieldFilter> filters = [
            new (FilterType.Equal, null, InboxEntryField.Id, expected.Id),
            // new (FilterType.Equal, null, InboxEntryField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, InboxEntryField.IdentityId, expected.IdentityId),
            new (FilterType.Equal, null, InboxEntryField.AddingDate, expected.AddingDate),
            new (FilterType.Equal, null, InboxEntryField.AvailableCommands, expected.AvailableCommands),
        ];

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesInboxEntriesDeleteCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new {processId, filters}, deleteResult);

        // Assert

        Assert.AreEqual(1, deleteResult.DeletedCount);
        Assert.IsNull(await repository.GetAsync(expected.Id));

        foreach (var model in models.Where(m => m.Id != expected.Id))
        {
            var notDeletedModel = await repository.GetAsync(model.Id);
            InboxEntryHelper.AssertModels(model, notDeletedModel, true);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<InboxEntryFieldFilter> filters = [
            new (FilterType.Equal, null, InboxEntryField.Id, models[0].Id),
            // new (FilterType.Equal, null, InboxEntryField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, InboxEntryField.IdentityId, models[1].IdentityId),
            new (FilterType.Equal, null, InboxEntryField.AddingDate, models[2].AddingDate),
            new (FilterType.Equal, null, InboxEntryField.AvailableCommands, models[3].AvailableCommands),
        ];

        List<InboxEntryFieldFilter> orFilters = [new InboxEntryFieldFilter(FilterType.Or, filters)];

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesInboxEntriesDeleteCollectionAsync(processId, filters: orFilters);

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
            InboxEntryHelper.AssertModels(models[i], notDeletedModel, true);
        }
    }

    [ClientTest] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<InboxEntryFieldFilter> filters = [
            new (FilterType.Equal, null, InboxEntryField.Id, models[0].Id),
            // new (FilterType.Equal, null, InboxEntryField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, InboxEntryField.IdentityId, models[1].IdentityId),
            new (FilterType.Equal, null, InboxEntryField.AddingDate, models[2].AddingDate),
            new (FilterType.Equal, null, InboxEntryField.AvailableCommands, models[3].AvailableCommands),
        ];

        var notFilters = filters.Select(f => new InboxEntryFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesInboxEntriesDeleteCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new {processId, filters = notFilters}, deleteResult);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            InboxEntryHelper.AssertModels(models[i], notDeletedModel, true);
        }

        for (int i = filters.Count; i < models.Length; i++)
        {
            Assert.IsNull(await repository.GetAsync(models[i].Id));
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = await InboxEntryHelper.ExclusivePermissionsApi(service, "delete-collection");

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = InboxEntryHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesInboxEntriesDeleteCollectionAsync(processId);

        TestLogger.LogApiCalled(new {processId}, deleteResult);

        // Assert

        Assert.IsTrue(deleteResult.DeletedCount == models.Length);

        foreach (var model in models)
        {
            var result = await repository.GetAsync(model.Id);
            Assert.IsNull(result);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await InboxEntryHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesInboxEntriesDeleteCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}