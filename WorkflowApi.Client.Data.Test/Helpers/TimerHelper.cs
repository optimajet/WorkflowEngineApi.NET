using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Helpers;

public static class TimerHelper
{
    private static Random Random => new();
    private static DateTime Now => new((DateTime.Now.Ticks / 10000000 + Random.Next(Int32.MinValue, Int32.MaxValue)) * 10000000);

    public static TimerModel Generate(Guid processId, bool log = true)
    {
        var model = new TimerModel(
            Guid.NewGuid(),
            processId,
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            Now,
            Guid.NewGuid().GetHashCode() % 2 == 0
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }

    public static TimerModel[] Generate(Guid processId, int count)
    {
        var models = new TimerModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(processId, false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static List<TimerCreateRequestWithName> CreateRequests(params TimerModel[] models)
    {
        List<TimerCreateRequestWithName> requests = [];

        foreach (var model in models)
        {
            requests.Add(new TimerCreateRequestWithName(
                model.Name,
                model.RootProcessId,
                model.NextExecutionDateTime,
                model.Ignore
            ));
        }

        return requests;
    }

    public static TimerCreateRequest CreateRequest(TimerModel model)
    {
        return new TimerCreateRequest(model.RootProcessId, model.NextExecutionDateTime, model.Ignore);
    }

    public static void AssertModels(TimerModel expected, TimerModel? actual, bool assertId = false)
    {
        Assert.IsNotNull(actual);
        if (assertId) Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.ProcessId, actual.ProcessId);
        Assert.AreEqual(expected.RootProcessId, actual.RootProcessId);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.NextExecutionDateTime, actual.NextExecutionDateTime);
        Assert.AreEqual(expected.Ignore, actual.Ignore);
    }

    public static TimerUpdateRequest UpdateRequest(TimerModel model, bool log = true)
    {
        var request = new TimerUpdateRequest(
            Guid.NewGuid().ToString(),
            Now,
            !model.Ignore
        );

        if (log) TestLogger.LogUpdateModelsGenerated([request], r => r.Name);

        return request;
    }

    public static List<TimerUpdateRequest> UpdateRequests(params TimerModel[] models)
    {
        List<TimerUpdateRequest> requests = [];

        foreach (var model in models)
        {
            requests.Add(UpdateRequest(model, false));
        }

        TestLogger.LogUpdateModelsGenerated(requests, r => r.Name);

        return requests;
    }

    public static void AssertUpdated(TimerUpdateRequest expected, TimerModel? actual)
    {
        TestLogger.LogAssertingUpdateModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.NextExecutionDateTime, actual.NextExecutionDateTime);
        Assert.AreEqual(expected.Ignore, actual.Ignore);
    }

    public static async Task<TimersApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Timers;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.processes.timers." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<TimersApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Timers;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.processes.timers"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<TimersApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Timers;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}