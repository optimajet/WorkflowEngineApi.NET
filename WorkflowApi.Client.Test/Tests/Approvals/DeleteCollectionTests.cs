using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Client;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Helpers;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests.Approvals;

[TestClass]
public class DeleteCollectionTests
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

        var extraModel = ApprovalHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesApprovalsDeleteCollectionAsync(processId);

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
        var repository = service.Repository.Approvals;
        var api = service.Client.Approvals;

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 20);

        await repository.CreateAsync(models);

        var expected = models.First();

        List<ApprovalFieldFilter> filters = [
            new (FilterType.Equal, null, ApprovalField.Id, expected.Id),
            new (FilterType.Equal, null, ApprovalField.ProcessId, expected.ProcessId),
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

        var deleteResult = await api.WorkflowApiDataProcessesApprovalsDeleteCollectionAsync(processId, filters: filters);

        TestLogger.LogApiCalled(new {processId, filters}, deleteResult);

        // Assert

        Assert.AreEqual(1, deleteResult.DeletedCount);
        Assert.IsNull(await repository.GetAsync(expected.Id));

        foreach (var model in models.Where(m => m.Id != expected.Id))
        {
            var notDeletedModel = await repository.GetAsync(model.Id);
            ApprovalHelper.AssertModels(model, notDeletedModel, true);
        }
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

        var deleteResult = await api.WorkflowApiDataProcessesApprovalsDeleteCollectionAsync(processId, filters: orFilters);

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
            ApprovalHelper.AssertModels(models[i], notDeletedModel, true);
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

        var deleteResult = await api.WorkflowApiDataProcessesApprovalsDeleteCollectionAsync(processId, filters: notFilters);

        TestLogger.LogApiCalled(new {processId, filters = notFilters}, deleteResult);

        // Assert

        Assert.AreEqual(models.Length - filters.Count, deleteResult.DeletedCount);

        for (int i = 0; i < filters.Count; i++)
        {
            var notDeletedModel = await repository.GetAsync(models[i].Id);
            ApprovalHelper.AssertModels(models[i], notDeletedModel, true);
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
        var repository = service.Repository.Approvals;
        var api = await ApprovalHelper.ExclusivePermissionsApi(service, "delete-collection");

        var processId = Guid.NewGuid();
        var models = ApprovalHelper.Generate(processId, 3);

        await repository.CreateAsync(models);

        var extraModel = ApprovalHelper.Generate(Guid.NewGuid());
        await repository.CreateAsync(extraModel);

        // Act

        var deleteResult = await api.WorkflowApiDataProcessesApprovalsDeleteCollectionAsync(processId);

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

        var api = await ApprovalHelper.NoPermissionsApi(service);

        // Act

        var exception = await Assert.ThrowsExceptionAsync<ApiException>(
            async () => await api.WorkflowApiDataProcessesApprovalsDeleteCollectionAsync(Guid.NewGuid())
        );

        // Assert

        Assert.AreEqual(403, exception.ErrorCode);
    }
}