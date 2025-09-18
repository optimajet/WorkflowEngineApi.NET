using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Helpers;

public static class ApprovalHelper
{
    private static Random Random => new();
    private static DateTime Now => new((DateTime.Now.Ticks / 10000000 + Random.Next(Int32.MinValue, Int32.MaxValue)) * 10000000);

    public static ApprovalModel Generate(Guid processId, bool log = true)
    {
        var model = new ApprovalModel(
            Guid.NewGuid(),
            processId,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString().Select(c => c.ToString()).ToList(),
            Now,
            Guid.NewGuid().GetHashCode(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }

    public static ApprovalModel[] Generate(Guid processId, int count)
    {
        var models = new ApprovalModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(processId, false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static void AssertModels(ApprovalModel expected, ApprovalModel? actual, bool assertId = false)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        if (assertId) Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.ProcessId, actual.ProcessId);
        Assert.AreEqual(expected.IdentityId, actual.IdentityId);
        CollectionAssert.AreEqual(expected.AllowedTo, actual.AllowedTo);
        Assert.AreEqual(expected.TransitionTime, actual.TransitionTime);
        Assert.AreEqual(expected.Sort, actual.Sort);
        Assert.AreEqual(expected.InitialState, actual.InitialState);
        Assert.AreEqual(expected.DestinationState, actual.DestinationState);
        Assert.AreEqual(expected.TriggerName, actual.TriggerName);
        Assert.AreEqual(expected.Commentary, actual.Commentary);
    }

    public static async Task<ApprovalsApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Approvals;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.processes.approvals." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<ApprovalsApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Approvals;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.processes.approvals"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<ApprovalsApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Approvals;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}