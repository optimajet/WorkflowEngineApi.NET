using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WorkflowApi.Swagger;

/// <summary>
/// An operation filter that adds security requirements to operations based on the presence of authorize attributes.
/// </summary>
public class SecurityOperationFilter : IOperationFilter
{
    public IOptions<AuthenticationOptions> AuthenticationOptions { get; }

    public SecurityOperationFilter(IOptions<AuthenticationOptions> authenticationOptions)
    {
        AuthenticationOptions = authenticationOptions;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var classAuthorize = context
            .MethodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .OfType<IAuthorizeData>()
            .FirstOrDefault();
        
        var methodAuthorize = context
            .MethodInfo
            .GetCustomAttributes(true)
            .OfType<IAuthorizeData>()
            .FirstOrDefault();
        
        var classAllowAnonymous = context
            .MethodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .OfType<IAllowAnonymous>()
            .Any() ?? false;
        
        var methodAllowAnonymous = context
            .MethodInfo
            .GetCustomAttributes(true)
            .OfType<IAllowAnonymous>()
            .Any();
        
        if (methodAllowAnonymous) return;
        if (classAllowAnonymous && methodAuthorize == null) return;
        
        var authorize = methodAuthorize ?? classAuthorize;
        if (authorize == null) return;

        var schemesString = authorize.AuthenticationSchemes 
                            ?? AuthenticationOptions.Value.DefaultAuthenticateScheme 
                            ?? AuthenticationOptions.Value.DefaultScheme;
        
        var schemes = schemesString?.Split(",") ?? [];
        
        var securityRequirements = new OpenApiSecurityRequirement();
        
        foreach (var scheme in schemes)
        {
            securityRequirements.Add(GetSecurityScheme(scheme), []);
        }
        
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            securityRequirements
        };
    }
    
    private OpenApiSecurityScheme GetSecurityScheme(string scheme)
    {
        return new()
        {
            Reference = new()
            {
                Type = ReferenceType.SecurityScheme,
                Id = scheme
            }
        };
    }
}