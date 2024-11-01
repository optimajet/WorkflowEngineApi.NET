using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WorkflowApi.Swagger;

/// <summary>
/// An operation filter that sets the operation ID based on a SwaggerOperationIdAttribute.
/// </summary>
public class OperationIdOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (String.IsNullOrWhiteSpace(operation.OperationId))
        {
            operation.OperationId = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<SwaggerOperationIdAttribute>()
                .Select(a => a.OperationId)
                .FirstOrDefault();
        }
    }
}