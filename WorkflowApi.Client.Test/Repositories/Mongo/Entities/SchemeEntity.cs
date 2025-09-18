using Newtonsoft.Json;
using WorkflowApi.Client.Test.Models;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

public class SchemeEntity
{
    public SchemeEntity() { }

    public SchemeEntity(Model.SchemeModel model)
    {
        Id = model.Code;
        Code = model.Code;
        Scheme = Converter.ToSchemaString(model.Scheme);
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

    public Model.SchemeModel ToModel()
    {
        return new Model.SchemeModel(
            Code,
            JsonConvert.DeserializeObject<WorkflowApi.Client.Model.Scheme>(JsonConvert.SerializeObject(Converter.FromSchemaString(Scheme))),
            CanBeInlined,
            InlinedSchemes,
            Tags
        );
    }
}
