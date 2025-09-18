using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Data.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Tests.InboxEntries;

[TestClass]
public class SearchTests
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

        var query = new InboxEntryFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);
        
        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            InboxEntryHelper.AssertModels(models[i], actual, true);
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

        var query = new InboxEntryFieldQuery(
            search: models.First().IdentityId
        );

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        InboxEntryHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().Id;

        var query = new InboxEntryFieldQuery
        {
            Filters = [new(FilterType.Equal, null, InboxEntryField.Id, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.Id);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithProcessIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().ProcessId;

        var query = new InboxEntryFieldQuery
        {
            Filters = [new(FilterType.Equal, null, InboxEntryField.ProcessId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.ProcessId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithIdentityIdEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().IdentityId;

        var query = new InboxEntryFieldQuery
        {
            Filters = [new(FilterType.Equal, null, InboxEntryField.IdentityId, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.IdentityId);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAddingDateEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().AddingDate;

        var query = new InboxEntryFieldQuery
        {
            Filters = [new(FilterType.Equal, null, InboxEntryField.AddingDate, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            Assert.AreEqual(expected, model.AddingDate);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAvailableCommandsEqualTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var expected = models.First().AvailableCommands;

        var query = new InboxEntryFieldQuery
        {
            Filters = [new(FilterType.Equal, null, InboxEntryField.AvailableCommands, expected)],
            Take = 100
        };

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= 1);
        Assert.IsTrue(result.Collection.Count >= 1);

        foreach (var model in result.Collection)
        {
            CollectionAssert.AreEqual(expected, model.AvailableCommands);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.InboxEntries;
        var api = service.Client.InboxEntries;

        var models = Enumerable.Range(0, 20).Select(_ => InboxEntryHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<InboxEntryFieldFilter> filters = [
            new (FilterType.Equal, null, InboxEntryField.Id, expected.Id),
            new (FilterType.Equal, null, InboxEntryField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, InboxEntryField.IdentityId, expected.IdentityId),
            new (FilterType.Equal, null, InboxEntryField.AddingDate, expected.AddingDate),
            new (FilterType.Equal, null, InboxEntryField.AvailableCommands, expected.AvailableCommands),
        ];

        // Act

        var query = new InboxEntryFieldQuery(filters: filters);
        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var models = Enumerable.Range(0, 20).Select(_ => InboxEntryHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        var expected = models.First();

        List<InboxEntryFieldFilter> filters = [
            new (FilterType.Equal, null, InboxEntryField.Id, expected.Id),
            new (FilterType.Equal, null, InboxEntryField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, InboxEntryField.IdentityId, expected.IdentityId),
            new (FilterType.Equal, null, InboxEntryField.AddingDate, expected.AddingDate),
            new (FilterType.Equal, null, InboxEntryField.AvailableCommands, expected.AvailableCommands),
        ];

        // Act

        var query = new InboxEntryFieldQuery(filters: [new InboxEntryFieldFilter(FilterType.And, filters)]);
        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var models = Enumerable.Range(0, 20).Select(_ => InboxEntryHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<InboxEntryFieldFilter> filters = [
            new (FilterType.Equal, null, InboxEntryField.Id, models[0].Id),
            new (FilterType.Equal, null, InboxEntryField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, InboxEntryField.IdentityId, models[2].IdentityId),
            new (FilterType.Equal, null, InboxEntryField.AddingDate, models[3].AddingDate),
            new (FilterType.Equal, null, InboxEntryField.AvailableCommands, models[4].AvailableCommands),
        ];

        // Act

        var query = new InboxEntryFieldQuery(filters: [new InboxEntryFieldFilter(FilterType.Or, filters)]);
        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

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

        var models = Enumerable.Range(0, 20).Select(_ => InboxEntryHelper.Generate(Guid.NewGuid())).ToArray();

        await repository.CreateAsync(models);

        List<InboxEntryFieldFilter> filters = [
            new (FilterType.Equal, null, InboxEntryField.Id, models[0].Id),
            new (FilterType.Equal, null, InboxEntryField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, InboxEntryField.IdentityId, models[2].IdentityId),
            new (FilterType.Equal, null, InboxEntryField.AddingDate, models[3].AddingDate),
            new (FilterType.Equal, null, InboxEntryField.AvailableCommands, models[4].AvailableCommands),
        ];

        var notFilters = filters.Select(f => new InboxEntryFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var query = new InboxEntryFieldQuery(filters: notFilters);
        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

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

        var filter = new InboxEntryFieldFilter(FilterType.Equal, null, InboxEntryField.ProcessId, processId);
        var sort = new InboxEntryFieldSort(InboxEntryField.AddingDate, Direction.Asc);

        // Act

        var queryPage1 = new InboxEntryFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 0
        );

        var page1 = await api.WorkflowApiSearchProcessesInboxEntriesAsync(queryPage1);

        TestLogger.LogApiCalled(queryPage1, page1);

        var queryPage2 = new InboxEntryFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 10
        );

        var page2 = await api.WorkflowApiSearchProcessesInboxEntriesAsync(queryPage2);

        TestLogger.LogApiCalled(queryPage2, page2);

        var queryPage3 = new InboxEntryFieldQuery(
            filters: [filter],
            sorts: [sort],
            take: 10,
            skip: 20
        );

        var page3 = await api.WorkflowApiSearchProcessesInboxEntriesAsync(queryPage3);

        TestLogger.LogApiCalled(queryPage3, page3);

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
        var api = await InboxEntryHelper.ExclusiveSearchPermissionsApi(service);

        var processId = Guid.NewGuid();
        var models = InboxEntryHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var query = new InboxEntryFieldQuery();

        // Act

        var result = await api.WorkflowApiSearchProcessesInboxEntriesAsync(query);

        TestLogger.LogApiCalled(query, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);

        for (int i = 0; i < models.Length; i++)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == models[i].Id);
            InboxEntryHelper.AssertModels(models[i], actual, true);
        }
    }

    [ClientTest(HostId.DataHost)]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await InboxEntryHelper.NoPermissionsApi(service);

        var query = new InboxEntryFieldQuery();

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiSearchProcessesInboxEntriesAsync(query)
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}