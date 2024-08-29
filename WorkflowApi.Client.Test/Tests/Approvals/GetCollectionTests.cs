using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Approvals;

[TestClass]
public class GetCollectionTests
{
    [ClientTest]
    [TestMethod]
    public async Task ExecuteTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);
        await repository.CreateAsync(ApprovalHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId);
        
        TestLogger.LogApiCalled(new {processId}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            ApprovalHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithSearchTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, search: models.First().IdentityId);

        TestLogger.LogApiCalled(new {processId, search = models.First().IdentityId}, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        var collection = result.Collection.ToList();

        Assert.AreEqual(1, collection.Count);
        var actual = collection.First();

        ApprovalHelper.AssertModels(models.First(), actual, true);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFiltersTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, expected.Id),
            // new (FilterType.Equal, null, ApprovalField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, expected.IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, expected.AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, expected.TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, expected.Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, expected.InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, expected.DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, expected.TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, expected.Commentary)
        ];

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new {processId, filters}, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ApprovalHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithAndFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ApprovalFieldFilter> filters = [new ApprovalFieldFilter(FilterType.And, [
            new (FilterType.Equal, null, ApprovalField.Id, expected.Id),
            // new (FilterType.Equal, null, ApprovalField.ProcessId, expected.ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, expected.IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, expected.AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, expected.TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, expected.Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, expected.InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, expected.DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, expected.TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, expected.Commentary)
        ])];

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new {processId, filters}, result);

        // Assert

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(1, result.Collection.Count);
        ApprovalHelper.AssertModels(expected, result.Collection.First(), true);
    }

    [ClientTest]
    [TestMethod]
    public async Task ExecuteWithOrFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, models[0].Id),
            // new (FilterType.Equal, null, ApprovalField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, models[1].IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, models[2].AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, models[3].TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, models[4].Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, models[5].InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, models[6].DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, models[7].TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, models[8].Commentary)
        ];

        List<ApprovalFieldFilter> orFilters = [new ApprovalFieldFilter(FilterType.Or, filters)];

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, filters: orFilters);

        TestLogger.LogApiCalled(new {processId, orFilters}, result);

        // Assert

        Assert.AreEqual(filters.Count, result.Total);
        Assert.AreEqual(filters.Count, result.Collection.Count);

        foreach (var model in result.Collection)
        {
            var expectedModel = models.First(x => x.Id == model.Id);
            ApprovalHelper.AssertModels(expectedModel, model, true);
        }
    }

    [ClientTest] 
    [TestMethod]
    public async Task ExecuteWithAndNotFilterTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, models[0].Id),
            // new (FilterType.Equal, null, ApprovalField.ProcessId, models[1].ProcessId),
            new (FilterType.Equal, null, ApprovalField.IdentityId, models[1].IdentityId),
            new (FilterType.Equal, null, ApprovalField.AllowedTo, models[2].AllowedTo),
            new (FilterType.Equal, null, ApprovalField.TransitionTime, models[3].TransitionTime),
            new (FilterType.Equal, null, ApprovalField.Sort, models[4].Sort),
            new (FilterType.Equal, null, ApprovalField.InitialState, models[5].InitialState),
            new (FilterType.Equal, null, ApprovalField.DestinationState, models[6].DestinationState),
            new (FilterType.Equal, null, ApprovalField.TriggerName, models[7].TriggerName),
            new (FilterType.Equal, null, ApprovalField.Commentary, models[8].Commentary)
        ];

        var notFilters = filters.Select(f => new ApprovalFieldFilter(FilterType.Not, [f])).ToList();

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, filters: notFilters);

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
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 30);

        await repository.CreateAsync(models);

        List<ApprovalFieldSort> sorts = [new ApprovalFieldSort(ApprovalField.Id, Direction.Asc)];

        // Act

        var page1 = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 0);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 0}, page1);

        var page2 = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 10);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 10}, page2);

        var page3 = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId, sorts: sorts, take: 10, skip: 20);

        TestLogger.LogApiCalled(new {processId, sorts, take = 10, skip = 20}, page3);

        ApprovalModelGetCollectionResponse[] pages = [page1, page2, page3];

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
                ApprovalHelper.AssertModels(expected, model, true);
            }
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionAllowedTest(TestService service)
    {
        // Arrange

        await using var context = service.Repository.Use();
        var repository = service.Repository.Approvals;
        var api = await ApprovalHelper.ExclusivePermissionsApi(service, "get-collection");

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);
        await repository.CreateAsync(ApprovalHelper.Generate(Guid.NewGuid()));

        // Act

        var result = await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(processId);
        
        TestLogger.LogApiCalled(new {processId}, result);

        // Assert

        Assert.IsTrue(result.Total >= models.Length);
        Assert.AreEqual(models.Length, result.Collection.Count);

        foreach (var model in models)
        {
            var actual = result.Collection.FirstOrDefault(x => x.Id == model.Id);
            ApprovalHelper.AssertModels(model, actual, true);
        }
    }

    [ClientTest]
    [TestMethod]
    public async Task PermissionDeniedTest(TestService service)
    {
        // Arrange

        var api = await ApprovalHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesApprovalsGetCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}