using Newtonsoft.Json;
using OptimaJet.DataEngine.Attributes;
using OptimaJet.Workflow.Api.Models;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowScheme")]
public class SchemeEntity
{
    public SchemeEntity() { }

    public SchemeEntity(WorkflowApi.Client.Model.SchemeModel model)
    {
        Code = model.Code;
        Scheme = Converter.ToSchemaString(
            JsonConvert.DeserializeObject<Scheme>(JsonConvert.SerializeObject(model.Scheme))
            ?? throw new InvalidOperationException("Failed to deserialize scheme from model.")
        );
        CanBeInlined = model.CanBeInlined;
        InlinedSchemes = Converter.ToJsonString(model.InlinedSchemes);
        Tags = Converter.ToTagString(model.Tags);
    }

    [PrimaryKey]
    [DataRequired]
    [DataLength(256)]
    public string Code { get; set; } = null!;

    [DataRequired]
    public string Scheme { get; set; } = null!;

    public bool CanBeInlined { get; set; }
    
    public string? InlinedSchemes { get; set; }
    
    public string? Tags { get; set; }

    public WorkflowApi.Client.Model.SchemeModel ToModel()
    {
        return new WorkflowApi.Client.Model.SchemeModel(
            Code,
            JsonConvert.DeserializeObject<WorkflowApi.Client.Model.Scheme>(JsonConvert.SerializeObject(Converter.FromSchemaString(Scheme))),
            CanBeInlined,
            Converter.FromJsonStringList(InlinedSchemes),
            Converter.FromJsonStringList(Tags)
        );
    }
}
