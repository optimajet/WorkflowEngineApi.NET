using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Helpers;

public static class ProcessHelper
{
    private static Random Random => new();
    private static DateTime Now => new((DateTime.Now.Ticks / 10000000 + Random.Next(Int32.MinValue, Int32.MaxValue)) * 10000000);

    public static ProcessModel Generate(bool log = true)
    {
        var model = new ProcessModel(
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid().GetHashCode() % 2 == 0,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Now,
            Now,
            Guid.NewGuid().ToString()
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }

    public static ProcessModel[] Generate(int count)
    {
        var models = new ProcessModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static void AssertModels(ProcessModel expected, ProcessModel? actual)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.StateName, actual.StateName);
        Assert.AreEqual(expected.ActivityName, actual.ActivityName);
        Assert.AreEqual(expected.SchemeId, actual.SchemeId);
        Assert.AreEqual(expected.PreviousState, actual.PreviousState);
        Assert.AreEqual(expected.PreviousStateForDirect, actual.PreviousStateForDirect);
        Assert.AreEqual(expected.PreviousStateForReverse, actual.PreviousStateForReverse);
        Assert.AreEqual(expected.PreviousActivity, actual.PreviousActivity);
        Assert.AreEqual(expected.PreviousActivityForDirect, actual.PreviousActivityForDirect);
        Assert.AreEqual(expected.PreviousActivityForReverse, actual.PreviousActivityForReverse);
        Assert.AreEqual(expected.ParentProcessId, actual.ParentProcessId);
        Assert.AreEqual(expected.RootProcessId, actual.RootProcessId);
        Assert.AreEqual(expected.IsDeterminingParametersChanged, actual.IsDeterminingParametersChanged);
        Assert.AreEqual(expected.TenantId, actual.TenantId);
        Assert.AreEqual(expected.StartingTransition, actual.StartingTransition);
        Assert.AreEqual(expected.SubprocessName, actual.SubprocessName);
        Assert.AreEqual(expected.CreationDate, actual.CreationDate);
        Assert.AreEqual(expected.LastTransitionDate, actual.LastTransitionDate);
        Assert.AreEqual(expected.CalendarName, actual.CalendarName);
    }

    public static ProcessUpdateRequest UpdateRequest(ProcessModel model, bool log = true)
    {
        var request = new ProcessUpdateRequest(
            model.Id.ToString(),
            Guid.NewGuid().ToString()
        );

        if (log) TestLogger.LogModelsGenerated([request], r => r.TenantId);

        return request;
    }

    public static List<ProcessUpdateRequest> UpdateRequests(params ProcessModel[] models)
    {
        List<ProcessUpdateRequest> requests = [];

        foreach (var model in models)
        {
            requests.Add(UpdateRequest(model, false));
        }

        TestLogger.LogUpdateModelsGenerated(requests, r => r.TenantId);

        return requests;
    }

    public static void AssertUpdated(ProcessUpdateRequest expected, ProcessModel? actual)
    {
        TestLogger.LogAssertingUpdateModels(expected, actual);

        Assert.IsNotNull(actual);
    }

    public static async Task<ProcessesApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Processes;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.processes." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<ProcessesApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Processes;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.processes"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<ProcessesApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Processes;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}