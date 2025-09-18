using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Helpers;

public static class InboxEntryHelper
{
    private static Random Random => new();
    private static DateTime Now => new((DateTime.Now.Ticks / 10000000 + Random.Next(Int32.MinValue, Int32.MaxValue)) * 10000000);

    public static InboxEntryModel Generate(Guid processId, bool log = true)
    {
        var model = new InboxEntryModel(
            Guid.NewGuid(),
            processId,
            Guid.NewGuid().ToString(),
            Now,
            Guid.NewGuid().ToString().Select(c => c.ToString()).ToList()
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }

    public static InboxEntryModel[] Generate(Guid processId, int count)
    {
        var models = new InboxEntryModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(processId, false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static void AssertModels(InboxEntryModel expected, InboxEntryModel? actual, bool assertId = false)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        if (assertId) Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.ProcessId, actual.ProcessId);
        Assert.AreEqual(expected.IdentityId, actual.IdentityId);
        Assert.AreEqual(expected.AddingDate, actual.AddingDate);
        CollectionAssert.AreEqual(expected.AvailableCommands, actual.AvailableCommands);
    }

    public static async Task<InboxEntriesApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.InboxEntries;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.processes.inbox-entries." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<InboxEntriesApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.InboxEntries;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.processes.inbox-entries"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<InboxEntriesApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.InboxEntries;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}