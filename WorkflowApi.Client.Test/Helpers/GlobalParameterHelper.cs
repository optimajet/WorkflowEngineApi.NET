using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Helpers;

public static class GlobalParameterHelper
{
    public static GlobalParameterModel Generate(bool log = true)
    {
        var model = new GlobalParameterModel(
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }

    public static GlobalParameterModel[] Generate(int count)
    {
        var models = new GlobalParameterModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static List<GlobalParameterCreateRequestWithTypeName> CreateRequests(params GlobalParameterModel[] models)
    {
        List<GlobalParameterCreateRequestWithTypeName> requests = [];

        foreach (var model in models)
        {
            requests.Add(new GlobalParameterCreateRequestWithTypeName(model.Type, model.Name, model.Value));
        }

        return requests;
    }

    public static GlobalParameterCreateRequest CreateRequest(GlobalParameterModel model)
    {
        return new GlobalParameterCreateRequest(model.Value);
    }

    public static void AssertModels(GlobalParameterModel expected, GlobalParameterModel? actual, bool assertId = false)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        if (assertId) Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.Type, actual.Type);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.Value, actual.Value);
    }

    public static GlobalParameterUpdateRequest UpdateRequest(GlobalParameterModel model, bool log = true)
    {
        var request = new GlobalParameterUpdateRequest(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );

        if (log) TestLogger.LogUpdateModelsGenerated([request], r => r.Name);

        return request;
    }

    public static List<GlobalParameterUpdateRequest> UpdateRequests(params GlobalParameterModel[] models)
    {
        List<GlobalParameterUpdateRequest> requests = [];

        foreach (var model in models)
        {
            requests.Add(UpdateRequest(model, false));
        }

        TestLogger.LogUpdateModelsGenerated(requests, r => r.Name);

        return requests;
    }

    public static void AssertUpdated(GlobalParameterUpdateRequest expected, GlobalParameterModel? actual)
    {
        TestLogger.LogAssertingUpdateModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Type, actual.Type);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.Value, actual.Value);
    }

    public static async Task<GlobalParametersApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.GlobalParameters;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.global-parameters." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<GlobalParametersApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.GlobalParameters;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.global-parameters"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<GlobalParametersApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.GlobalParameters;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }
}