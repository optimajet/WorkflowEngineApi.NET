using Newtonsoft.Json;
using OptimaJet.Workflow.Api.Models;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

public class SchemeEntity
{
    public SchemeEntity() { }

    public SchemeEntity(WorkflowApi.Client.Model.SchemeModel model)
    {
        Id = model.Code;
        Code = model.Code;
        Scheme = Converter.ToSchemaString(
            JsonConvert.DeserializeObject<Scheme>(JsonConvert.SerializeObject(model.Scheme))
            ?? throw new InvalidOperationException("Failed to deserialize scheme from model.")
        );
        CanBeInlined = model.CanBeInlined;
        InlinedSchemes = model.InlinedSchemes;
        Tags = Converter.SimplifyTags(model.Tags);
    }

    public string Id { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Scheme { get; set; } = null!;
    public bool CanBeInlined { get; set; }
    public List<string> InlinedSchemes { get; set; } = new();
    public List<string> Tags { get; set; } = new();

    public WorkflowApi.Client.Model.SchemeModel ToModel()
    {
        return new WorkflowApi.Client.Model.SchemeModel(
            Code,
            JsonConvert.DeserializeObject<WorkflowApi.Client.Model.Scheme>(JsonConvert.SerializeObject(Converter.FromSchemaString(Scheme))),
            CanBeInlined,
            InlinedSchemes,
            Tags
        );
    }
}
