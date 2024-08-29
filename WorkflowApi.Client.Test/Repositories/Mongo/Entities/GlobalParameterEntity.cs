using OptimaJet.Workflow.Api.Models;
using GlobalParameterModel = WorkflowApi.Client.Model.GlobalParameterModel;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

public class GlobalParameterEntity
{
    public GlobalParameterEntity() { }

    public GlobalParameterEntity(GlobalParameterModel model)
    {
        Id = model.Id;
        Type = model.Type;
        Name = model.Name;
        Value = Converter.ToJsonString(model.Value);
    }

    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;

    public GlobalParameterModel ToModel()
    {
        return new GlobalParameterModel(
            Id,
            Type,
            Name,
            Converter.FromJsonObject(Value)
        );
    }
}
