using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Helpers;

public static class TransitionHelper
{
    private static Random Random => new();
    private static DateTime Now => new((DateTime.Now.Ticks / 10000000 + Random.Next(Int32.MinValue, Int32.MaxValue)) * 10000000);

    public static TransitionModel Generate(Guid processId, bool log = true)
    {
        var model = new TransitionModel(
            Guid.NewGuid(),
            processId,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Now,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().GetHashCode() % 2 == 0,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Now,
            Guid.NewGuid().GetHashCode()
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }

    public static TransitionModel[] Generate(Guid processId, int count)
    {
        var models = new TransitionModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(processId, false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static void AssertModels(TransitionModel expected, TransitionModel? actual, bool assertId = false)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        if (assertId) Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.ProcessId, actual.ProcessId);
        Assert.AreEqual(expected.ExecutorIdentityId, actual.ExecutorIdentityId);
        Assert.AreEqual(expected.ActorIdentityId, actual.ActorIdentityId);
        Assert.AreEqual(expected.ExecutorName, actual.ExecutorName);
        Assert.AreEqual(expected.ActorName, actual.ActorName);
        Assert.AreEqual(expected.FromActivityName, actual.FromActivityName);
        Assert.AreEqual(expected.ToActivityName, actual.ToActivityName);
        Assert.AreEqual(expected.ToStateName, actual.ToStateName);
        Assert.AreEqual(expected.TransitionTime, actual.TransitionTime);
        Assert.AreEqual(expected.TransitionClassifier, actual.TransitionClassifier);
        Assert.AreEqual(expected.IsFinalised, actual.IsFinalised);
        Assert.AreEqual(expected.FromStateName, actual.FromStateName);
        Assert.AreEqual(expected.TriggerName, actual.TriggerName);
        Assert.AreEqual(expected.StartTransitionTime, actual.StartTransitionTime);
        Assert.AreEqual(expected.TransitionDuration, actual.TransitionDuration);
    }

    public static async Task<TransitionsApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Transitions;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.processes.transitions." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<TransitionsApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Transitions;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.processes.transitions"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<TransitionsApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Transitions;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}