using WorkflowApi.Client.Test.Models;
using ParameterModel = WorkflowApi.Client.Model.ParameterModel;

namespace WorkflowApi.Client.Test.Repositories.Mongo.Entities;

public class ParameterEntity
{
    public ParameterEntity() { }

    public ParameterEntity(ParameterModel model)
    {
        ParameterName = model.Name;
        Value = Converter.ToJsonString(model.Value);
    }

    public string ParameterName { get; set; } = null!;
    public string Value { get; set; } = null!;

    public ParameterModel ToModel(Guid processId)
    {
        return new ParameterModel(
            default,
            processId,
            ParameterName,
            Converter.FromJsonObject(Value)
        );
    }
}
