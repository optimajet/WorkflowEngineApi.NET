using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.InboxEntries;

[TestClass]
public class GetCollectionTests
{
    [ClientTest(HostId.DataHost)]
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
        await repository.CreateAsync(InboxEntryHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            InboxEntryHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, search: models.First().IdentityId);

        TestLogger.LogApiCalled(new { processId, models.First().IdentityId }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        InboxEntryHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
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

        var result = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new { processId, filters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        InboxEntryHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
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

        List<InboxEntryFieldFilter> andFilters = [new InboxEntryFieldFilter(FilterType.And, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, filters: andFilters);

        TestLogger.LogApiCalled(new { processId, andFilters }, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        InboxEntryHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest(HostId.DataHost)]
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

        var result = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, filters: orFilters);

        TestLogger.LogApiCalled(new { processId, orFilters }, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            InboxEntryHelper.AssertModels(expectedModel, model, true);
        }
    }

    [ClientTest(HostId.DataHost)] 
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

        var result = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new { processId, notFilters }, result);

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
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        List<InboxEntryFieldSort> sorts = [new InboxEntryFieldSort(InboxEntryField.AddingDate, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new { processId, sorts, take = 10, skip = 0 }, page1);

        var page2 = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new { processId, sorts, take = 10, skip = 10 }, page2);

        var page3 = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new { processId, sorts, take = 10, skip = 20 }, page3);

        InboxEntryModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                InboxEntryHelper.AssertModels(expected, model, true);
            }
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = await InboxEntryHelper.ExclusivePermissionsApi(service, "get-collection");

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);
        await repository.CreateAsync(InboxEntryHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(processId);

        TestLogger.LogApiCalled(new { processId }, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            InboxEntryHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await InboxEntryHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesInboxEntriesGetCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}