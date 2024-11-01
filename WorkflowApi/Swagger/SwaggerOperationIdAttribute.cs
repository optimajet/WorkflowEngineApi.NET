namespace WorkflowApi.Swagger;

/// <summary>
/// An attribute that sets the operation ID for a Swagger specification.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class SwaggerOperationIdAttribute : Attribute
{
    /// <summary>
    /// Creates a new SwaggerOperationIdAttribute with the specified operation ID.
    /// </summary>
    /// <param name="operationId"></param>
    public SwaggerOperationIdAttribute(string operationId)
    {
        OperationId = operationId;
    }

    /// <summary>
    /// The operation ID for the Swagger specification.
    /// </summary>
    public string OperationId { get; }
}
