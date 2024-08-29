using OptimaJet.DataEngine.Attributes;
using OptimaJet.Workflow.Api.Models;
using GlobalParameterModel = WorkflowApi.Client.Model.GlobalParameterModel;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowGlobalParameter")]
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

    [PrimaryKey]
    public Guid Id { get; set; }
    
    [DataRequired]
    [DataLength(306)]
    public string Type { get; set; } = null!;

    [DataRequired]
    [DataLength(128)]
    public string Name { get; set; } = null!;

    [DataRequired]
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
