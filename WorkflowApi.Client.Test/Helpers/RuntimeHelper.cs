using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Helpers;

public static class RuntimeHelper
{
    private static Random Random => new();
    private static DateTime Now => new((DateTime.Now.Ticks / 10000000 + Random.Next(Int32.MinValue, Int32.MaxValue)) * 10000000);

    public static RuntimeModel Generate(bool log = true)
    {
        var model = new RuntimeModel(
            Guid.NewGuid().ToString(),
            Guid.NewGuid(),
            (RuntimeStatus) Random.Next(1, 6),
            Guid.NewGuid().ToString(),
            Now,
            Now,
            Now
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id);

        return model;
    }

    public static RuntimeModel[] Generate(int count)
    {
        var models = new RuntimeModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id);

        return models;
    }

    public static void AssertModels(RuntimeModel expected, RuntimeModel? actual)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.VarLock, actual.VarLock);
        Assert.AreEqual(expected.Status, actual.Status);
        Assert.AreEqual(expected.RestorerId, actual.RestorerId);
        Assert.AreEqual(expected.NextTimerTime, actual.NextTimerTime);
        Assert.AreEqual(expected.NextServiceTimerTime, actual.NextServiceTimerTime);
        Assert.AreEqual(expected.LastAliveSignal, actual.LastAliveSignal);
    }

    public static async Task<RuntimesApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Runtimes;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.runtimes." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<RuntimesApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Runtimes;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.runtimes"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<RuntimesApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Runtimes;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}