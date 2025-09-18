using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Data.Test.Helpers;

public static class ParameterHelper
{
    public static ParameterModel CreateModel(Guid processId, string name, object value, bool log = true)
    {
        var model = new ParameterModel(
            Guid.NewGuid(),
            processId,
            name,
            value
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }
    
    public static ParameterModel Generate(Guid processId, bool log = true)
    {
        var model = new ParameterModel(
            Guid.NewGuid(),
            processId,
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Id.ToString());

        return model;
    }

    public static ParameterModel[] Generate(Guid processId, int count)
    {
        var models = new ParameterModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(processId, false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Id.ToString());

        return models;
    }

    public static List<ParameterCreateRequestWithName> CreateRequests(params ParameterModel[] models)
    {
        List<ParameterCreateRequestWithName> requests = [];

        foreach (var model in models)
        {
            requests.Add(new ParameterCreateRequestWithName(model.Name, model.Value));
        }

        return requests;
    }

    public static ParameterCreateRequest CreateRequest(ParameterModel model)
    {
        return new ParameterCreateRequest(model.Value);
    }

    public static void AssertModels(ParameterModel expected, ParameterModel? actual, bool assertId = false)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        if (assertId) Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.ProcessId, actual.ProcessId);
        Assert.AreEqual(expected.Name, actual.Name);
        AssertParameterValue(expected.Value, actual.Value);
    }
    
    public static ParameterUpdateRequest UpdateRequest(ParameterModel model, bool log = true)
    {
        var request = new ParameterUpdateRequest(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );

        if (log) TestLogger.LogUpdateModelsGenerated([model], m => m.Id.ToString());

        return request;
    }

    public static List<ParameterUpdateRequest> UpdateRequests(params ParameterModel[] models)
    {
        List<ParameterUpdateRequest> requests = [];

        foreach (var model in models)
        {
            requests.Add(UpdateRequest(model, false));
        }

        TestLogger.LogUpdateModelsGenerated(models, m => m.Id.ToString());

        return requests;
    }

    public static void AssertUpdated(ParameterUpdateRequest expected, ParameterModel? actual)
    {
        TestLogger.LogAssertingUpdateModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Name, actual.Name);
        AssertParameterValue(expected.Value, actual.Value);
    }

    public static async Task<ParametersApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Parameters;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.processes.parameters." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<ParametersApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Parameters;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.processes.parameters"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<ParametersApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Parameters;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    private static void AssertParameterValue(object expected, object actual)
    {
        var expectedJson = JsonConvert.SerializeObject(ConvertKeysToCamelCase(expected));
        var actualJson = JsonConvert.SerializeObject(ConvertKeysToCamelCase(actual));

        Assert.AreEqual(expectedJson, actualJson);
    }
    
    private static JToken ConvertKeysToCamelCase(object value)
    {
        var token = value as JToken ?? JToken.FromObject(value);
        
        switch (token)
        {
            case JObject jObject:
            {
                var newJObject = new JObject();
                var namingStrategy = new CamelCaseNamingStrategy();

                foreach (var property in jObject.Properties())
                {
                    var newPropertyName = namingStrategy.GetPropertyName(property.Name, false);
                    newJObject[newPropertyName] = ConvertKeysToCamelCase(property.Value);
                }

                return newJObject;
            }
            case JArray array:
                return new JArray(array.Select(ConvertKeysToCamelCase));
            default:
                return token; // primitive value
        }
    }
}
