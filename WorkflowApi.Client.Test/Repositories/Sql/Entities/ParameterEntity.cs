using OptimaJet.DataEngine.Attributes;
using OptimaJet.Workflow.Api.Models;
using ParameterModel = WorkflowApi.Client.Model.ParameterModel;

namespace WorkflowApi.Client.Test.Repositories.Sql.Entities;

[TableName("WorkflowProcessInstancePersistence")]
public class ParameterEntity
{
    public ParameterEntity() { }

    public ParameterEntity(ParameterModel model)
    {
        Id = model.Id;
        ProcessId = model.ProcessId;
        ParameterName = model.Name;
        Value = Converter.ToJsonString(model.Value);
    }

    [PrimaryKey]
    public Guid Id { get; set; }
    
    public Guid ProcessId { get; set; }
    
    [DataRequired]
    public string ParameterName { get; set; } = null!;

    [DataRequired]
    public string Value { get; set; } = null!;

    public ParameterModel ToModel()
    {
        return new ParameterModel(
            Id,
            ProcessId,
            ParameterName,
            Converter.FromJsonObject(Value)
        );
    }
}
