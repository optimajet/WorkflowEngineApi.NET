using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WorkflowApi.Swagger;

/// <summary>
/// A document filter that adds security requirements to operations based on endpoint metadata.
/// </summary>
public class SecurityOperationFilter : IDocumentFilter
{
    public IOptions<AuthenticationOptions> AuthenticationOptions { get; }

    public SecurityOperationFilter(IOptions<AuthenticationOptions> authenticationOptions)
    {
        AuthenticationOptions = authenticationOptions;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var defaultSchemes = GetSchemes(null);

        foreach (var apiDescription in context.ApiDescriptions)
        {
            var path = "/" + apiDescription.RelativePath!.TrimStart('/');
            if (path.StartsWith("/auth", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var endpointMetadata = apiDescription.ActionDescriptor.EndpointMetadata ?? [];
            if (endpointMetadata.OfType<IAllowAnonymous>().Any())
            {
                continue;
            }

            var authorize = endpointMetadata.OfType<IAuthorizeData>().FirstOrDefault();
            if (!swaggerDoc.Paths.TryGetValue(path, out var pathItem))
            {
                continue;
            }

            if (pathItem.Operations == null)
            {
                continue;
            }

            var operation = pathItem.Operations
                .FirstOrDefault(pair => String.Equals(pair.Key.ToString(), apiDescription.HttpMethod, StringComparison.OrdinalIgnoreCase))
                .Value;

            if (operation == null)
            {
                continue;
            }

            var securityRequirements = new OpenApiSecurityRequirement();
            foreach (var scheme in GetSchemes(authorize))
            {
                securityRequirements.Add(new OpenApiSecuritySchemeReference(scheme, swaggerDoc, null), []);
            }

            operation.Security = [securityRequirements];
        }

        foreach (var pathItem in swaggerDoc.Paths)
        {
            if (pathItem.Key.StartsWith("/auth", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (pathItem.Value.Operations == null)
            {
                continue;
            }

            foreach (var operation in pathItem.Value.Operations.Values)
            {
                if (operation.Security != null && operation.Security.Count > 0)
                {
                    continue;
                }

                var securityRequirements = new OpenApiSecurityRequirement();
                foreach (var scheme in defaultSchemes)
                {
                    securityRequirements.Add(new OpenApiSecuritySchemeReference(scheme, swaggerDoc, null), []);
                }

                operation.Security = [securityRequirements];
            }
        }
    }

    private string[] GetSchemes(IAuthorizeData? authorize)
    {
        var schemes = SplitSchemes(authorize?.AuthenticationSchemes);

        if (schemes.Length > 0)
        {
            return schemes;
        }

        schemes = SplitSchemes(AuthenticationOptions.Value.DefaultAuthenticateScheme);
        if (schemes.Length > 0)
        {
            return schemes;
        }

        return SplitSchemes(AuthenticationOptions.Value.DefaultScheme);
    }

    private static string[] SplitSchemes(string? schemes)
    {
        return schemes?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(scheme => !String.IsNullOrWhiteSpace(scheme))
            .ToArray()
            ?? [];
    }
}
