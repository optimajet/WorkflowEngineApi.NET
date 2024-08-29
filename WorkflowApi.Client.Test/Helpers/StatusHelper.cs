using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Helpers;

public static class StatusHelper
{
    private static Random Random => new();
    private static DateTime Now => new((DateTime.Now.Ticks / 10000000 + Random.Next(Int32.MinValue, Int32.MaxValue)) * 10000000);

    public static StatusModel Generate(bool log = true)
    {
        var models = new StatusModel(
            Guid.NewGuid(),
            Guid.NewGuid().ToByteArray().First() / 2,
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            Now
        );

        if (log) TestLogger.LogModelsGenerated([models], m => m.Id.ToString());

        return models;
    }

    public static StatusModel[] Generate(int count)
    {
        var models = new StatusModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static void AssertModels(StatusModel expected, StatusModel? actual)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.StatusCode, actual.StatusCode);
        Assert.AreEqual(expected.VarLock, actual.VarLock);
        Assert.AreEqual(expected.RuntimeId, actual.RuntimeId);
        Assert.AreEqual(expected.SetTime, actual.SetTime);
    }

    public static async Task<StatusesApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Statuses;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.statuses." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<StatusesApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Statuses;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.statuses"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<StatusesApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Statuses;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}