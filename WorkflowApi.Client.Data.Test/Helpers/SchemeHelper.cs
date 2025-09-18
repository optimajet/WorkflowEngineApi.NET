using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Test.Models;
using WorkflowApi.Client.Test.Runner;
using SchemeCreateRequest = WorkflowApi.Client.Model.SchemeCreateRequest;
using SchemeCreateRequestWithCode = WorkflowApi.Client.Model.SchemeCreateRequestWithCode;
using SchemeModel = WorkflowApi.Client.Model.SchemeModel;
using SchemeUpdateRequest = WorkflowApi.Client.Model.SchemeUpdateRequest;

namespace WorkflowApi.Client.Data.Test.Helpers;

public static class SchemeHelper
{
    public static SchemeModel Generate(bool log = true)
    {
        var model = new SchemeModel(
            Guid.NewGuid().ToString(),
            GenerateScheme(),
            Guid.NewGuid().GetHashCode() % 2 == 0,
            Guid.NewGuid().ToString().Select(c => c.ToString()).ToList(),
            Guid.NewGuid().ToString().Select(c => c.ToString()).ToList()
        );

        if (log) TestLogger.LogModelsGenerated([model], m => m.Code);

        return model;
    }

    public static SchemeModel[] Generate(int count)
    {
        var models = new SchemeModel[count];

        for (int i = 0; i < count; i++)
        {
            models[i] = Generate(false);
        }

        TestLogger.LogModelsGenerated(models, model => model.Code);

        return models;
    }

    public static List<SchemeCreateRequestWithCode> CreateRequests(params SchemeModel[] models)
    {
        List<SchemeCreateRequestWithCode> requests = [];

        foreach (var model in models)
        {
            requests.Add(new SchemeCreateRequestWithCode(
                model.Code,
                model.Scheme,
                model.CanBeInlined,
                model.InlinedSchemes,
                model.Tags
            ));
        }

        return requests;
    }

    public static SchemeCreateRequest CreateRequest(SchemeModel model)
    {
        return new SchemeCreateRequest(
            model.Scheme,
            model.CanBeInlined,
            model.InlinedSchemes,
            model.Tags
        );
    }

    public static void AssertModels(SchemeModel expected, SchemeModel? actual)
    {
        TestLogger.LogAssertingModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Code, actual.Code);
        // Scheme Cant be asserted because it's generated object
        Assert.AreEqual(expected.CanBeInlined, actual.CanBeInlined);
        CollectionAssert.AreEqual(expected.InlinedSchemes, actual.InlinedSchemes);
        CollectionAssert.AreEqual(Converter.SimplifyTags(expected.Tags), actual.Tags);
    }

    public static SchemeUpdateRequest UpdateRequest(SchemeModel model, bool log = true)
    {
        var request = new SchemeUpdateRequest(
            Guid.NewGuid().ToString(),
            GenerateScheme(),
            !model.CanBeInlined,
            Guid.NewGuid().ToString().Select(c => c.ToString()).ToList(),
            Guid.NewGuid().ToString().Select(c => c.ToString()).ToList()
        );

        if (log) TestLogger.LogUpdateModelsGenerated([model], m => m.Code);

        return request;
    }

    public static List<SchemeUpdateRequest> UpdateRequests(params SchemeModel[] models)
    {
        List<SchemeUpdateRequest> requests = [];

        foreach (var model in models)
        {
            requests.Add(UpdateRequest(model, false));
        }

        TestLogger.LogUpdateModelsGenerated(models, m => m.Code);

        return requests;
    }

    public static void AssertUpdated(SchemeUpdateRequest expected, SchemeModel? actual)
    {
        TestLogger.LogAssertingUpdateModels(expected, actual);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Code, actual.Code);
        //Scheme cant be asserted because it's generated object
        Assert.AreEqual(expected.CanBeInlined, actual.CanBeInlined);
        CollectionAssert.AreEqual(expected.InlinedSchemes, actual.InlinedSchemes);
        CollectionAssert.AreEqual(Converter.SimplifyTags(expected.Tags), actual.Tags);
        CollectionAssert.AreEqual(Converter.SimplifyTags(expected.Tags), actual.Scheme.Tags);
    }

    public static async Task<SchemesApi> ExclusivePermissionsApi(TestService service, string method)
    {
        var api = service.Client.Schemes;
        var token = await service.Client.CreateTokenAsync(["workflow-api.data.schemes." + method]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<SchemesApi> ExclusiveSearchPermissionsApi(TestService service)
    {
        var api = service.Client.Schemes;
        var token = await service.Client.CreateTokenAsync(["workflow-api.search.schemes"]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    public static async Task<SchemesApi> NoPermissionsApi(TestService service)
    {
        var api = service.Client.Schemes;
        var token = await service.Client.CreateTokenAsync([]);
        api.Configuration.DefaultHeaders["Authorization"] = $"Bearer {JsonConvert.DeserializeObject<string>(token)}";
        return api;
    }

    private static WorkflowApi.Client.Model.Scheme GenerateScheme()
    {
        var scheme = JsonConvert.DeserializeObject<WorkflowApi.Client.Model.Scheme>(Scheme)
                     ?? throw new InvalidOperationException("Failed to deserialize scheme.");
        scheme.Name = Guid.NewGuid().ToString();
        return scheme;
    }

    public const string Scheme = """
                                 {
                                   "Actors": [],
                                   "Parameters": [
                                     {
                                       "Type": "Guid",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ProcessId",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "PreviousState",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "CurrentState",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "PreviousStateForDirect",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "PreviousStateForReverse",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "PreviousActivity",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "PreviousActivityForDirect",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "PreviousActivityForReverse",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "CurrentCommand",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "IdentityId",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ImpersonatedIdentityId",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ExecutedActivityState",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "OptimaJet.Workflow.Core.Model.ActivityDefinition",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ExecutedActivity",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "OptimaJet.Workflow.Core.Model.TransitionDefinition",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ExecutedTransition",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "CurrentActivity",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "Guid",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "SchemeId",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "System.Collections.Generic.IEnumerable%3CString%3E",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "IdentityIds",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "SchemeCode",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "Boolean",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "IsPreExecution",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ExecutedTimer",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "System.Collections.Generic.IEnumerable%3CString%3E",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "IdentityIdsForCurrentActivity",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "Guid",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ParentProcessId",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "Guid",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "RootProcessId",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "StartTransitionalProcessActivity",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "TenantId",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "String",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "SubprocessName",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "DateTime",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "CreationDate",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "DateTime%3F",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "LastTransitionDate",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "Boolean",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "ProcessActivityChangedSent",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "Type": "OptimaJet.Workflow.Core.Model.ProcessInstance",
                                       "Purpose": 2,
                                       "InitialValue": null,
                                       "Name": "PreviousProcessInstance",
                                       "DesignerSettings": {
                                         "X": null,
                                         "Y": null,
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     }
                                   ],
                                   "Commands": [],
                                   "Timers": [],
                                   "Comments": [],
                                   "Activities": [
                                     {
                                       "OriginalName": "Activity_1",
                                       "OriginalSchemeCode": "AutoTransitions",
                                       "LastTimeInlineName": null,
                                       "FirstTimeInlineName": null,
                                       "WasInlined": true,
                                       "ActivityType": 0,
                                       "SchemeCode": null,
                                       "State": null,
                                       "IsInitial": true,
                                       "IsFinal": false,
                                       "IsForSetState": true,
                                       "IsAutoSchemeUpdate": true,
                                       "DisablePersistState": false,
                                       "DisablePersistTransitionHistory": false,
                                       "DisablePersistParameters": false,
                                       "UserComment": null,
                                       "HaveImplementation": false,
                                       "HavePreExecutionImplementation": false,
                                       "Implementation": [],
                                       "PreExecutionImplementation": [],
                                       "Annotations": [],
                                       "ExecutionTimeout": null,
                                       "IdleTimeout": null,
                                       "ExceptionsHandlers": [],
                                       "IsState": false,
                                       "Name": "Activity_1",
                                       "DesignerSettings": {
                                         "X": "444",
                                         "Y": "279",
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "OriginalName": "Activity_2",
                                       "OriginalSchemeCode": "AutoTransitions",
                                       "LastTimeInlineName": null,
                                       "FirstTimeInlineName": null,
                                       "WasInlined": true,
                                       "ActivityType": 0,
                                       "SchemeCode": null,
                                       "State": null,
                                       "IsInitial": false,
                                       "IsFinal": false,
                                       "IsForSetState": true,
                                       "IsAutoSchemeUpdate": true,
                                       "DisablePersistState": false,
                                       "DisablePersistTransitionHistory": false,
                                       "DisablePersistParameters": false,
                                       "UserComment": null,
                                       "HaveImplementation": false,
                                       "HavePreExecutionImplementation": false,
                                       "Implementation": [],
                                       "PreExecutionImplementation": [],
                                       "Annotations": [],
                                       "ExecutionTimeout": null,
                                       "IdleTimeout": null,
                                       "ExceptionsHandlers": [],
                                       "IsState": false,
                                       "Name": "Activity_2",
                                       "DesignerSettings": {
                                         "X": "744",
                                         "Y": "279",
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "OriginalName": "Activity_3",
                                       "OriginalSchemeCode": "AutoTransitions",
                                       "LastTimeInlineName": null,
                                       "FirstTimeInlineName": null,
                                       "WasInlined": true,
                                       "ActivityType": 0,
                                       "SchemeCode": null,
                                       "State": null,
                                       "IsInitial": false,
                                       "IsFinal": true,
                                       "IsForSetState": true,
                                       "IsAutoSchemeUpdate": true,
                                       "DisablePersistState": false,
                                       "DisablePersistTransitionHistory": false,
                                       "DisablePersistParameters": false,
                                       "UserComment": null,
                                       "HaveImplementation": false,
                                       "HavePreExecutionImplementation": false,
                                       "Implementation": [],
                                       "PreExecutionImplementation": [],
                                       "Annotations": [],
                                       "ExecutionTimeout": null,
                                       "IdleTimeout": null,
                                       "ExceptionsHandlers": [],
                                       "IsState": false,
                                       "Name": "Activity_3",
                                       "DesignerSettings": {
                                         "X": "1044",
                                         "Y": "279",
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     }
                                   ],
                                   "Transitions": [
                                     {
                                       "InlinedFinalActivityName": null,
                                       "OriginalName": null,
                                       "OriginalSchemeCode": null,
                                       "LastTimeInlineName": null,
                                       "FirstTimeInlineName": null,
                                       "UserComment": null,
                                       "WasInlined": false,
                                       "From": {
                                         "OriginalName": "Activity_1",
                                         "OriginalSchemeCode": "AutoTransitions",
                                         "LastTimeInlineName": null,
                                         "FirstTimeInlineName": null,
                                         "WasInlined": true,
                                         "ActivityType": 0,
                                         "SchemeCode": null,
                                         "State": null,
                                         "IsInitial": true,
                                         "IsFinal": false,
                                         "IsForSetState": true,
                                         "IsAutoSchemeUpdate": true,
                                         "DisablePersistState": false,
                                         "DisablePersistTransitionHistory": false,
                                         "DisablePersistParameters": false,
                                         "UserComment": null,
                                         "HaveImplementation": false,
                                         "HavePreExecutionImplementation": false,
                                         "Implementation": [],
                                         "PreExecutionImplementation": [],
                                         "Annotations": [],
                                         "ExecutionTimeout": null,
                                         "IdleTimeout": null,
                                         "ExceptionsHandlers": [],
                                         "IsState": false,
                                         "Name": "Activity_1",
                                         "DesignerSettings": {
                                           "X": "444",
                                           "Y": "279",
                                           "Bending": null,
                                           "Scale": null,
                                           "Color": null,
                                           "AutoTextContrast": true,
                                           "Group": null,
                                           "Hidden": false,
                                           "OverwriteActivityTo": null
                                         }
                                       },
                                       "To": {
                                         "OriginalName": "Activity_2",
                                         "OriginalSchemeCode": "AutoTransitions",
                                         "LastTimeInlineName": null,
                                         "FirstTimeInlineName": null,
                                         "WasInlined": true,
                                         "ActivityType": 0,
                                         "SchemeCode": null,
                                         "State": null,
                                         "IsInitial": false,
                                         "IsFinal": false,
                                         "IsForSetState": true,
                                         "IsAutoSchemeUpdate": true,
                                         "DisablePersistState": false,
                                         "DisablePersistTransitionHistory": false,
                                         "DisablePersistParameters": false,
                                         "UserComment": null,
                                         "HaveImplementation": false,
                                         "HavePreExecutionImplementation": false,
                                         "Implementation": [],
                                         "PreExecutionImplementation": [],
                                         "Annotations": [],
                                         "ExecutionTimeout": null,
                                         "IdleTimeout": null,
                                         "ExceptionsHandlers": [],
                                         "IsState": false,
                                         "Name": "Activity_2",
                                         "DesignerSettings": {
                                           "X": "744",
                                           "Y": "279",
                                           "Bending": null,
                                           "Scale": null,
                                           "Color": null,
                                           "AutoTextContrast": true,
                                           "Group": null,
                                           "Hidden": false,
                                           "OverwriteActivityTo": null
                                         }
                                       },
                                       "Classifier": 0,
                                       "Trigger": {
                                         "Type": 1,
                                         "NameRef": "",
                                         "Command": null,
                                         "Timer": null
                                       },
                                       "Conditions": [
                                         {
                                           "Type": 1,
                                           "Action": null,
                                           "Expression": null,
                                           "ResultOnPreExecution": null,
                                           "ConditionInversion": false
                                         }
                                       ],
                                       "Restrictions": [],
                                       "AllowConcatenationType": 0,
                                       "RestrictConcatenationType": 0,
                                       "ConditionsConcatenationType": 0,
                                       "Annotations": [],
                                       "IsFork": false,
                                       "MergeViaSetState": false,
                                       "DisableParentStateControl": false,
                                       "SubprocessStartupType": 0,
                                       "SubprocessInOutDefinition": 0,
                                       "SubprocessName": null,
                                       "SubprocessId": null,
                                       "SubprocessStartupParameterCopyStrategy": 0,
                                       "SubprocessFinalizeParameterMergeStrategy": 0,
                                       "SubprocessSpecifiedParameters": null,
                                       "SubprocessSpecifiedParametersList": [],
                                       "IsAlwaysTransition": true,
                                       "IsOtherwiseTransition": false,
                                       "IsConditionTransition": false,
                                       "ForkType": 0,
                                       "Name": "Activity_1_Activity_2_1",
                                       "DesignerSettings": {
                                         "X": "686",
                                         "Y": "309",
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     },
                                     {
                                       "InlinedFinalActivityName": null,
                                       "OriginalName": null,
                                       "OriginalSchemeCode": null,
                                       "LastTimeInlineName": null,
                                       "FirstTimeInlineName": null,
                                       "UserComment": null,
                                       "WasInlined": false,
                                       "From": {
                                         "OriginalName": "Activity_2",
                                         "OriginalSchemeCode": "AutoTransitions",
                                         "LastTimeInlineName": null,
                                         "FirstTimeInlineName": null,
                                         "WasInlined": true,
                                         "ActivityType": 0,
                                         "SchemeCode": null,
                                         "State": null,
                                         "IsInitial": false,
                                         "IsFinal": false,
                                         "IsForSetState": true,
                                         "IsAutoSchemeUpdate": true,
                                         "DisablePersistState": false,
                                         "DisablePersistTransitionHistory": false,
                                         "DisablePersistParameters": false,
                                         "UserComment": null,
                                         "HaveImplementation": false,
                                         "HavePreExecutionImplementation": false,
                                         "Implementation": [],
                                         "PreExecutionImplementation": [],
                                         "Annotations": [],
                                         "ExecutionTimeout": null,
                                         "IdleTimeout": null,
                                         "ExceptionsHandlers": [],
                                         "IsState": false,
                                         "Name": "Activity_2",
                                         "DesignerSettings": {
                                           "X": "744",
                                           "Y": "279",
                                           "Bending": null,
                                           "Scale": null,
                                           "Color": null,
                                           "AutoTextContrast": true,
                                           "Group": null,
                                           "Hidden": false,
                                           "OverwriteActivityTo": null
                                         }
                                       },
                                       "To": {
                                         "OriginalName": "Activity_3",
                                         "OriginalSchemeCode": "AutoTransitions",
                                         "LastTimeInlineName": null,
                                         "FirstTimeInlineName": null,
                                         "WasInlined": true,
                                         "ActivityType": 0,
                                         "SchemeCode": null,
                                         "State": null,
                                         "IsInitial": false,
                                         "IsFinal": true,
                                         "IsForSetState": true,
                                         "IsAutoSchemeUpdate": true,
                                         "DisablePersistState": false,
                                         "DisablePersistTransitionHistory": false,
                                         "DisablePersistParameters": false,
                                         "UserComment": null,
                                         "HaveImplementation": false,
                                         "HavePreExecutionImplementation": false,
                                         "Implementation": [],
                                         "PreExecutionImplementation": [],
                                         "Annotations": [],
                                         "ExecutionTimeout": null,
                                         "IdleTimeout": null,
                                         "ExceptionsHandlers": [],
                                         "IsState": false,
                                         "Name": "Activity_3",
                                         "DesignerSettings": {
                                           "X": "1044",
                                           "Y": "279",
                                           "Bending": null,
                                           "Scale": null,
                                           "Color": null,
                                           "AutoTextContrast": true,
                                           "Group": null,
                                           "Hidden": false,
                                           "OverwriteActivityTo": null
                                         }
                                       },
                                       "Classifier": 0,
                                       "Trigger": {
                                         "Type": 1,
                                         "NameRef": "",
                                         "Command": null,
                                         "Timer": null
                                       },
                                       "Conditions": [
                                         {
                                           "Type": 1,
                                           "Action": null,
                                           "Expression": null,
                                           "ResultOnPreExecution": null,
                                           "ConditionInversion": false
                                         }
                                       ],
                                       "Restrictions": [],
                                       "AllowConcatenationType": 0,
                                       "RestrictConcatenationType": 0,
                                       "ConditionsConcatenationType": 0,
                                       "Annotations": [],
                                       "IsFork": false,
                                       "MergeViaSetState": false,
                                       "DisableParentStateControl": false,
                                       "SubprocessStartupType": 0,
                                       "SubprocessInOutDefinition": 0,
                                       "SubprocessName": null,
                                       "SubprocessId": null,
                                       "SubprocessStartupParameterCopyStrategy": 0,
                                       "SubprocessFinalizeParameterMergeStrategy": 0,
                                       "SubprocessSpecifiedParameters": null,
                                       "SubprocessSpecifiedParametersList": [],
                                       "IsAlwaysTransition": true,
                                       "IsOtherwiseTransition": false,
                                       "IsConditionTransition": false,
                                       "ForkType": 0,
                                       "Name": "Activity_2_Activity_3_1",
                                       "DesignerSettings": {
                                         "X": "991",
                                         "Y": "309",
                                         "Bending": null,
                                         "Scale": null,
                                         "Color": null,
                                         "AutoTextContrast": true,
                                         "Group": null,
                                         "Hidden": false,
                                         "OverwriteActivityTo": null
                                       }
                                     }
                                   ],
                                   "Localization": [],
                                   "CodeActions": [],
                                   "CodeActionsCommonUsings": null,
                                   "AdditionalParams": {
                                     "Rules": [
                                       "CheckUser",
                                       "CheckRole"
                                     ],
                                     "RulesExcluded": [],
                                     "TimerTypes": [
                                       "Interval",
                                       "Time",
                                       "Date",
                                       "DateAndTime",
                                       "Expression"
                                     ],
                                     "Conditions": [],
                                     "ConditionsExcluded": [],
                                     "Actions": [
                                       "WriteProcessHistory",
                                       "UpdateProcessHistory",
                                       "ShowForm"
                                     ],
                                     "ActionsExcluded": [],
                                     "Usings": [
                                       "OptimaJet.WorkflowServer.Faults",
                                       "OptimaJet.WorkflowServer",
                                       "OptimaJet.WorkflowServer.Data.QueryBuilder.Glossaries",
                                       "OptimaJet.WorkflowServer.Model",
                                       "OptimaJet.WorkflowServer.Services.MySql",
                                       "Microsoft.CodeAnalysis",
                                       "OptimaJet.WorkflowServer.Identity",
                                       "OptimaJet.WorkflowServer.Data.Repositories.SchemesRepository",
                                       "OptimaJet.Logging",
                                       "OptimaJet.WorkflowServer.Identity.Sync",
                                       "OptimaJet.WorkflowServer.Services.MongoDB",
                                       "OptimaJet.Workflow.Core.Runtime",
                                       "System.Net.Http.HPack",
                                       "System.Net.Http.Headers",
                                       "OptimaJet.WorkflowServer.Integration.Base.Interfaces",
                                       "OptimaJet.WorkflowServer.Data.QueryBuilder",
                                       "OptimaJet.WorkflowServer.Model.MongoDB",
                                       "FxResources.System.Net.Http",
                                       "OptimaJet.WorkflowServer.Callback",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL",
                                       "OptimaJet.WorkflowServer.Plugins",
                                       "OptimaJet.WorkflowServer.Model.MSSQL",
                                       "Microsoft.Extensions.Internal",
                                       "System.Collections.Generic",
                                       "OptimaJet.WorkflowServer.Services.PostgreSql",
                                       "OptimaJet.WorkflowServer.Services.Cors",
                                       "OptimaJet.WorkflowServer.Forms.Model",
                                       "OptimaJet.Workflow",
                                       "OptimaJet.WorkflowServer.Data.Repositories.SchemesRepository.SqlProviders",
                                       "OptimaJet.WorkflowServer.Model.Entity",
                                       "OptimaJet.WorkflowServer.RuleProviders",
                                       "OptimaJet.WorkflowServer.Hubs",
                                       "OptimaJet.WorkflowServer.Identity.IdentityDataProvider",
                                       "Microsoft.CSharp",
                                       "OptimaJet.WorkflowServer.Model.Users",
                                       "System.IO",
                                       "OptimaJet.WorkflowServer.Data",
                                       "OptimaJet.WorkflowServer.Services.MsSql",
                                       "OptimaJet.WorkflowServer.Logging",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel",
                                       "System.Threading",
                                       "OptimaJet.WorkflowServer.Services.Oracle",
                                       "OptimaJet.WorkflowServer.Initializing",
                                       "System.Threading.Tasks",
                                       "System.Runtime.CompilerServices",
                                       "System.Net.Security",
                                       "Microsoft.CSharp.RuntimeBinder",
                                       "System.Collections",
                                       "OptimaJet.WorkflowServer.Services",
                                       "OptimaJet.WorkflowServer.BackgoundTasks",
                                       "OptimaJet.WorkflowServer.License",
                                       "OptimaJet.WorkflowServer.Identity.Native",
                                       "System.Net.Http.QPack",
                                       "OptimaJet.WorkflowServer.Utils",
                                       "OptimaJet.Workflow.Core.Model",
                                       "System",
                                       "OptimaJet.WorkflowServer.Forms",
                                       "OptimaJet.WorkflowServer.Model.Oracle",
                                       "OptimaJet.WorkflowServer.Integration.Base.Classes",
                                       "OptimaJet.WorkflowServer.Api.Config",
                                       "System.Net.Http",
                                       "System.Text",
                                       "System.Net",
                                       "System.Dynamic",
                                       "OptimaJet.WorkflowServer.Model.MySQL",
                                       "OptimaJet.Logging.Database",
                                       "OptimaJet.WorkflowServer.Api",
                                       "OptimaJet.WorkflowServer.Persistence",
                                       "System.Linq",
                                       "OptimaJet.WorkflowServer.Data.Repositories"
                                     ],
                                     "Types": [
                                       "MySQLProviderExtensions",
                                       "OracleProviderExtensions",
                                       "PostgreSQLProviderExtensions",
                                       "WorkflowServerPostgreSQLProvider",
                                       "OptimaJet.Logging.ProcessLogger",
                                       "OptimaJet.Logging.Database.ProcessLog",
                                       "OptimaJet.WorkflowServer.ConfigApiOperations",
                                       "OptimaJet.WorkflowServer.ConfigApiLoggingOperations",
                                       "OptimaJet.WorkflowServer.RuntimesOperations",
                                       "OptimaJet.WorkflowServer.ConfigApiWorkflowOperations",
                                       "OptimaJet.WorkflowServer.ConfigApiFormOperations",
                                       "OptimaJet.WorkflowServer.ConfigApiUsersOperations",
                                       "OptimaJet.WorkflowServer.ConfigApiSections",
                                       "OptimaJet.WorkflowServer.ConfigApiObjectState",
                                       "OptimaJet.WorkflowServer.ConfigApi",
                                       "OptimaJet.WorkflowServer.Cryptography",
                                       "OptimaJet.WorkflowServer.GetInstancesResults",
                                       "OptimaJet.WorkflowServer.DatabaseOperationsInstanceOptions",
                                       "OptimaJet.WorkflowServer.DatabaseOperations",
                                       "OptimaJet.WorkflowServer.Extensions",
                                       "OptimaJet.WorkflowServer.HttpRequestsProcessor",
                                       "OptimaJet.WorkflowServer.CommandExecutionResult",
                                       "OptimaJet.WorkflowServer.ReportPeriods",
                                       "OptimaJet.WorkflowServer.ListSuccessResponse<>",
                                       "OptimaJet.WorkflowServer.ItemSuccessResponse<>",
                                       "OptimaJet.WorkflowServer.SuccessResponse",
                                       "OptimaJet.WorkflowServer.FailResponse",
                                       "OptimaJet.WorkflowServer.CallbackServerResponse<>",
                                       "OptimaJet.WorkflowServer.StreamResponse",
                                       "OptimaJet.WorkflowServer.SchemeExParameters",
                                       "OptimaJet.WorkflowServer.SchemeSettings",
                                       "OptimaJet.WorkflowServer.ServerResponse",
                                       "OptimaJet.WorkflowServer.JsonServerResponse",
                                       "OptimaJet.WorkflowServer.StringServerResponse",
                                       "OptimaJet.WorkflowServer.EmptyServerResponse",
                                       "OptimaJet.WorkflowServer.StreamFileServerResponse",
                                       "OptimaJet.WorkflowServer.FileServerResponse",
                                       "OptimaJet.WorkflowServer.RedirectServerResponse",
                                       "OptimaJet.WorkflowServer.ImplicitParametersParsingType",
                                       "OptimaJet.WorkflowServer.ImplicitParametersFromCallbackServerPersistenceBehavior",
                                       "OptimaJet.WorkflowServer.CallbackServerJsonSerialization",
                                       "OptimaJet.WorkflowServer.ProxySettings",
                                       "OptimaJet.WorkflowServer.CacheSettings",
                                       "OptimaJet.WorkflowServer.ServerSettings",
                                       "OptimaJet.WorkflowServer.UrlInfo",
                                       "OptimaJet.WorkflowServer.ServerTypes",
                                       "OptimaJet.WorkflowServer.StartupServerExtensions",
                                       "OptimaJet.WorkflowServer.WorkflowServerRuntime",
                                       "OptimaJet.WorkflowServer.WorkflowServerRuntimeExtensions",
                                       "OptimaJet.WorkflowServer.Initializing.WorkflowServerInitializer",
                                       "OptimaJet.WorkflowServer.Utils.LifetimeCacheRetrySettings",
                                       "OptimaJet.WorkflowServer.Utils.LifetimeCache<>",
                                       "OptimaJet.WorkflowServer.Services.MetadataCreatorBase<,>",
                                       "OptimaJet.WorkflowServer.Services.WorkflowProcessParametersProcessor",
                                       "OptimaJet.WorkflowServer.Services.PostgreSql.MetadataCreator",
                                       "OptimaJet.WorkflowServer.Services.Oracle.MetadataCreator",
                                       "OptimaJet.WorkflowServer.Services.MySql.MetadataCreator",
                                       "OptimaJet.WorkflowServer.Services.MsSql.MetadataCreator",
                                       "OptimaJet.WorkflowServer.Services.MongoDB.ReportService",
                                       "OptimaJet.WorkflowServer.Services.Cors.CorsManager",
                                       "OptimaJet.WorkflowServer.Services.Cors.CorsPolicyProvider",
                                       "OptimaJet.WorkflowServer.RuleProviders.IdentityRuleProvider",
                                       "OptimaJet.WorkflowServer.Persistence.Configuration",
                                       "OptimaJet.WorkflowServer.Persistence.ConfigurationSettings",
                                       "OptimaJet.WorkflowServer.Persistence.ExportSettings",
                                       "OptimaJet.WorkflowServer.Persistence.IncludeMetadataSubSettings",
                                       "OptimaJet.WorkflowServer.Persistence.IncludeUsersSubSettings",
                                       "OptimaJet.WorkflowServer.Persistence.ImportSettings",
                                       "OptimaJet.WorkflowServer.Persistence.FilePersistence",
                                       "OptimaJet.WorkflowServer.Persistence.Form",
                                       "OptimaJet.WorkflowServer.Persistence.LogPersistence",
                                       "OptimaJet.WorkflowServer.Persistence.Metadata",
                                       "OptimaJet.WorkflowServer.Persistence.SchemeExtension",
                                       "OptimaJet.WorkflowServer.Persistence.User",
                                       "OptimaJet.WorkflowServer.Model.BusinessFlow",
                                       "OptimaJet.WorkflowServer.Model.BusinessFlowMap",
                                       "OptimaJet.WorkflowServer.Model.CodeAction",
                                       "OptimaJet.WorkflowServer.Model.ConfigurationSettings",
                                       "OptimaJet.WorkflowServer.Model.CorsSettings",
                                       "OptimaJet.WorkflowServer.Model.DataModelUtils",
                                       "OptimaJet.WorkflowServer.Model.EntriesPage<>",
                                       "OptimaJet.WorkflowServer.Model.FormField",
                                       "OptimaJet.WorkflowServer.Model.Form",
                                       "OptimaJet.WorkflowServer.Model.FrontendCustomization",
                                       "OptimaJet.WorkflowServer.Model.CustomizationField",
                                       "OptimaJet.WorkflowServer.Model.LicenseInfo",
                                       "OptimaJet.WorkflowServer.Model.LogEntriesPage",
                                       "OptimaJet.WorkflowServer.Model.Metadata",
                                       "OptimaJet.WorkflowServer.Model.Plugin",
                                       "OptimaJet.WorkflowServer.Model.PublicUrl",
                                       "OptimaJet.WorkflowServer.Model.WorkflowReportBySchemes",
                                       "OptimaJet.WorkflowServer.Model.WorkflowReportByTransitions",
                                       "OptimaJet.WorkflowServer.Model.WorkflowReportByStats",
                                       "OptimaJet.WorkflowServer.Model.Role",
                                       "OptimaJet.WorkflowServer.Model.Security",
                                       "OptimaJet.WorkflowServer.Model.Settings",
                                       "OptimaJet.WorkflowServer.Model.StreamEntry",
                                       "OptimaJet.WorkflowServer.Model.TransitionItem",
                                       "OptimaJet.WorkflowServer.Model.Credential",
                                       "OptimaJet.WorkflowServer.Model.User",
                                       "OptimaJet.WorkflowServer.Model.WorkflowApi",
                                       "OptimaJet.WorkflowServer.Model.WorkflowProcessInstance",
                                       "OptimaJet.WorkflowServer.Model.WorkflowScheme",
                                       "OptimaJet.WorkflowServer.Model.WorkflowServerProcessHistoryItem",
                                       "OptimaJet.WorkflowServer.Model.Users.ColumnSort",
                                       "OptimaJet.WorkflowServer.Model.Users.ExternalLogin",
                                       "OptimaJet.WorkflowServer.Model.Users.UserEditModel",
                                       "OptimaJet.WorkflowServer.Model.Users.UserFilter",
                                       "OptimaJet.WorkflowServer.Model.Users.UserRequest",
                                       "OptimaJet.WorkflowServer.Model.Users.UserUpdate",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.ActionReference",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Activity",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Actor",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Annotation",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.CodeAction",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.CodeActionParameter",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Command",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Condition",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Option",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Parameter",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.ParameterReference",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Restriction",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Scheme",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Timer",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Transition",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Translation",
                                       "OptimaJet.WorkflowServer.Model.SchemeModel.Trigger",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL.WorkflowProcessLogs",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL.WorkflowSchemeEx",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL.WorkflowServerLogs",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL.WorkflowServerProcessHistory",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL.WorkflowServerStats",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL.WorkflowServerUser",
                                       "OptimaJet.WorkflowServer.Model.PostgreSQL.WorkflowServerUserCredential",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowProcessLogs",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowSchemeEx",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowServerLogs",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowServerOracleProvider",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowServerProcessHistory",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowServerStats",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowServerUser",
                                       "OptimaJet.WorkflowServer.Model.Oracle.WorkflowServerUserCredential",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowProcessLogs",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowSchemeEx",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowServerLogs",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowServerMySQLProvider",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowServerProcessHistory",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowServerStats",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowServerUser",
                                       "OptimaJet.WorkflowServer.Model.MySQL.WorkflowServerUserCredential",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.MSSQLProviderExtensions",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowProcessLogs",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowSchemeEx",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowServerLogs",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowServerMSSQLProvider",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowServerProcessHistory",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowServerStats",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowServerUser",
                                       "OptimaJet.WorkflowServer.Model.MSSQL.WorkflowServerUserCredential",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowInbox",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowOutbox",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowProcessLogs",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowSchemeEx",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowServerLogs",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowServerProcessHistory",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowServerStats",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowServerUser",
                                       "OptimaJet.WorkflowServer.Model.MongoDB.WorkflowServerUserCredential",
                                       "OptimaJet.WorkflowServer.Model.Entity.WorkflowProcessLogsEntity",
                                       "OptimaJet.WorkflowServer.Model.Entity.WorkflowSchemeExEntity",
                                       "OptimaJet.WorkflowServer.Model.Entity.WorkflowServerLogsEntity",
                                       "OptimaJet.WorkflowServer.Model.Entity.WorkflowServerProcessHistoryEntity",
                                       "OptimaJet.WorkflowServer.Model.Entity.WorkflowServerStatsEntity",
                                       "OptimaJet.WorkflowServer.Model.Entity.WorkflowServerUserCredentialEntity",
                                       "OptimaJet.WorkflowServer.Model.Entity.WorkflowServerUserEntity",
                                       "OptimaJet.WorkflowServer.Logging.LogPersistenceProvider",
                                       "OptimaJet.WorkflowServer.Logging.ProcessLogPersistenceProvider",
                                       "OptimaJet.WorkflowServer.Logging.SortDirections",
                                       "OptimaJet.WorkflowServer.Logging.Sort",
                                       "OptimaJet.WorkflowServer.Logging.SortColumns",
                                       "OptimaJet.WorkflowServer.Logging.TableFilter",
                                       "OptimaJet.WorkflowServer.Logging.TableRequest",
                                       "OptimaJet.WorkflowServer.License.LicenseException",
                                       "OptimaJet.WorkflowServer.License.BaseRestrictions",
                                       "OptimaJet.WorkflowServer.License.WorkflowServerRestrictions",
                                       "OptimaJet.WorkflowServer.License.LicenseCheckType",
                                       "OptimaJet.WorkflowServer.License.LicenseType",
                                       "OptimaJet.WorkflowServer.License.LicenseKey<>",
                                       "OptimaJet.WorkflowServer.License.Licensing",
                                       "OptimaJet.WorkflowServer.License.TimeHelper",
                                       "OptimaJet.WorkflowServer.Identity.AccessTokenSecretValidator",
                                       "OptimaJet.WorkflowServer.Identity.AccountRequestProcessor",
                                       "OptimaJet.WorkflowServer.Identity.AuthTypes",
                                       "OptimaJet.WorkflowServer.Identity.ExternalCredentialsProcessor",
                                       "OptimaJet.WorkflowServer.Identity.ExternalLoginProviderInfo",
                                       "OptimaJet.WorkflowServer.Identity.ExternalLogonService",
                                       "OptimaJet.WorkflowServer.Identity.HostRedirectUriValidator",
                                       "OptimaJet.WorkflowServer.Identity.Clients",
                                       "OptimaJet.WorkflowServer.Identity.SecretTypes",
                                       "OptimaJet.WorkflowServer.Identity.IdentityScopes",
                                       "OptimaJet.WorkflowServer.Identity.WfsClaimTypes",
                                       "OptimaJet.WorkflowServer.Identity.IdentityResources",
                                       "OptimaJet.WorkflowServer.Identity.AuthenticationSchemes",
                                       "OptimaJet.WorkflowServer.Identity.PolicyNames",
                                       "OptimaJet.WorkflowServer.Identity.AdminAccount",
                                       "OptimaJet.WorkflowServer.Identity.IdentityExtensions",
                                       "OptimaJet.WorkflowServer.Identity.IdentityServerConfig",
                                       "OptimaJet.WorkflowServer.Identity.IdentityServerError",
                                       "OptimaJet.WorkflowServer.Identity.IdentityServerErrorInfo",
                                       "OptimaJet.WorkflowServer.Identity.IdentityServerSettings",
                                       "OptimaJet.WorkflowServer.Identity.AuthOptions",
                                       "OptimaJet.WorkflowServer.Identity.IExternalCredentialsProcessor",
                                       "OptimaJet.WorkflowServer.Identity.IExternalLogonService",
                                       "OptimaJet.WorkflowServer.Identity.PasswordAuthentication",
                                       "OptimaJet.WorkflowServer.Identity.PasswordHelper",
                                       "OptimaJet.WorkflowServer.Identity.PersistenceParameters",
                                       "OptimaJet.WorkflowServer.Identity.SameHostRedirectUriValidator",
                                       "OptimaJet.WorkflowServer.Identity.SecuritySettings",
                                       "OptimaJet.WorkflowServer.Identity.SecurityTypes",
                                       "OptimaJet.WorkflowServer.Identity.UserProfileService",
                                       "OptimaJet.WorkflowServer.Identity.VoidHostRedirectUriValidator",
                                       "OptimaJet.WorkflowServer.Identity.Sync.DirectoryAccessSync",
                                       "OptimaJet.WorkflowServer.Identity.Sync.ImportDirectoryAccessResult",
                                       "OptimaJet.WorkflowServer.Identity.Sync.ImportDirectoryAccessUserResult",
                                       "OptimaJet.WorkflowServer.Identity.Sync.UserSync",
                                       "OptimaJet.WorkflowServer.Identity.IdentityDataProvider.IIdentityDataProvider",
                                       "OptimaJet.WorkflowServer.Identity.IdentityDataProvider.LdapAttributeMapping",
                                       "OptimaJet.WorkflowServer.Identity.IdentityDataProvider.LDAPConf",
                                       "OptimaJet.WorkflowServer.Identity.IdentityDataProvider.LdapDataProvider",
                                       "OptimaJet.WorkflowServer.Identity.IdentityDataProvider.LdapUtil",
                                       "OptimaJet.WorkflowServer.Identity.IdentityDataProvider.User",
                                       "OptimaJet.WorkflowServer.Hubs.AutocompleteHub",
                                       "OptimaJet.WorkflowServer.Hubs.MetadataHub",
                                       "OptimaJet.WorkflowServer.Forms.Authorization",
                                       "OptimaJet.WorkflowServer.Forms.DataController",
                                       "OptimaJet.WorkflowServer.Forms.AccessType",
                                       "OptimaJet.WorkflowServer.Forms.FlowController",
                                       "OptimaJet.WorkflowServer.Forms.RequestOptions",
                                       "OptimaJet.WorkflowServer.Forms.FormManager",
                                       "OptimaJet.WorkflowServer.Forms.FormOperations",
                                       "OptimaJet.WorkflowServer.Forms.ShowFormParameter",
                                       "OptimaJet.WorkflowServer.Forms.FormPlugin",
                                       "OptimaJet.WorkflowServer.Forms.Roles",
                                       "OptimaJet.WorkflowServer.Forms.UserController",
                                       "OptimaJet.WorkflowServer.Forms.WorkflowController",
                                       "OptimaJet.WorkflowServer.Forms.Model.ProfileUpdate",
                                       "OptimaJet.WorkflowServer.Faults.TenantIdAssertException",
                                       "OptimaJet.WorkflowServer.Callback.CallbackCommunication",
                                       "OptimaJet.WorkflowServer.Callback.CallbackEndpoint",
                                       "OptimaJet.WorkflowServer.Callback.CallbackEventType",
                                       "OptimaJet.WorkflowServer.Callback.CallbackException",
                                       "OptimaJet.WorkflowServer.Callback.CallbackLocator",
                                       "OptimaJet.WorkflowServer.Callback.CallbackMethod",
                                       "OptimaJet.WorkflowServer.Callback.CallbackMethodExtensions",
                                       "OptimaJet.WorkflowServer.Callback.CallbackMethodType",
                                       "OptimaJet.WorkflowServer.Callback.CallbackNameType",
                                       "OptimaJet.WorkflowServer.Callback.CallbackParcel",
                                       "OptimaJet.WorkflowServer.Callback.CallbackProvider",
                                       "OptimaJet.WorkflowServer.Callback.CallbackServerConfiguration",
                                       "OptimaJet.WorkflowServer.Callback.CallbackTest",
                                       "OptimaJet.WorkflowServer.Callback.ICallbackCommunication",
                                       "OptimaJet.WorkflowServer.BackgoundTasks.UpdateDashboardHostedService",
                                       "String",
                                       "Char",
                                       "Byte",
                                       "Int16",
                                       "Int32",
                                       "Int64",
                                       "Single",
                                       "Double",
                                       "Decimal",
                                       "DateTime"
                                     ],
                                     "CustomConditions": [],
                                     "MaxNumberOfActivities": -1,
                                     "MaxNumberOfTransitions": -1,
                                     "MaxNumberOfCommands": -1,
                                     "MaxNumberOfSchemes": -1,
                                     "MaxNumberOfThreads": -1,
                                     "InlinedSchemeCodes": []
                                   },
                                   "CanBeInlined": false,
                                   "LogEnabled": false,
                                   "InlinedSchemes": [],
                                   "StartingTransition": null,
                                   "Tags": [],
                                   "ParametersForSerialize": [],
                                   "PersistenceParameters": [],
                                   "ActorsForSerialize": [],
                                   "DefiningParametersString": null,
                                   "RootSchemeCode": null,
                                   "RootSchemeId": null,
                                   "IsObsolete": false,
                                   "Id": "00000000-0000-0000-0000-000000000000",
                                   "IsSubprocessScheme": false,
                                   "AllowedActivities": null,
                                   "Name": "AutoTransitions",
                                   "DesignerSettings": {
                                     "X": null,
                                     "Y": null,
                                     "Bending": null,
                                     "Scale": null,
                                     "Color": null,
                                     "AutoTextContrast": true,
                                     "Group": null,
                                     "Hidden": false,
                                     "OverwriteActivityTo": null
                                   }
                                 }
                                 """;
}