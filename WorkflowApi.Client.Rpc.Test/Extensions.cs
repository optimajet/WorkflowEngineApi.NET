using Newtonsoft.Json;
using OptimaJet.Workflow.Core.Model.Builder;
using WorkflowApi.Client.Api;
using WorkflowApi.Client.Model;
using WorkflowApi.Client.Test.Models;

namespace WorkflowApi.Client.Rpc.Test;

public static class Extensions
{
    public static Task CreateSchemeFromBuilderAsync(this ISchemesApi api, IProcessDefinitionBuilder schemeBuilder, List<string>? tags = null)
    {
        var processDefinition = schemeBuilder.ProcessDefinition;
        var scheme = Converter.FromSchemaString(processDefinition.Serialize());

        var request = new SchemeCreateRequest()
        {
            Scheme = JsonConvert.DeserializeObject<Model.Scheme>(JsonConvert.SerializeObject(scheme)),
            CanBeInlined = processDefinition.CanBeInlined,
            InlinedSchemes = processDefinition.InlinedSchemes,
            Tags = tags ?? processDefinition.Tags
        };
        
        return api.WorkflowApiDataSchemesCreateAsync(scheme.Name, request);
    }
}