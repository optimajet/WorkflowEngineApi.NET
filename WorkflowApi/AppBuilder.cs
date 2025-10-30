using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Api.Mongo;
using OptimaJet.Workflow.Api.Mssql;
using OptimaJet.Workflow.Api.Mysql;
using OptimaJet.Workflow.Api.Options;
using OptimaJet.Workflow.Api.Oracle;
using OptimaJet.Workflow.Api.Postgres;
using OptimaJet.Workflow.Api.Sqlite;
using OptimaJet.Workflow.Core.Runtime;
using Swashbuckle.AspNetCore.SwaggerGen;
using WorkflowApi.Data;
using WorkflowApi.Models;
using WorkflowApi.Swagger;

namespace WorkflowApi;

/// <summary>
/// A builder for the web application with pre-configured services and middleware.
/// </summary>
public class AppBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppBuilder"/> class.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public AppBuilder(params string[] args)
    {
        Builder = WebApplication.CreateBuilder(args);
    }

    /// <summary>
    /// Gets the web application builder.
    /// </summary>
    public WebApplicationBuilder Builder { get; }

    /// <summary>
    /// Gets the configuration of the application.
    /// </summary>
    public Configuration Configuration => Builder.Configuration.Get<Configuration>() ?? new();

    /// <summary>
    /// Configures pre-configured services of the application.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The data provider is not supported.</exception>
    public void ConfigureServices()
    {
        if (_servicesConfigured) return;
        
        // Default values from Configuration object being added to the configuration.
        Builder.Configuration.AddObject(Configuration);
        Builder.Configure<WorkflowApiCoreOptions>();
        Builder.Configure<WorkflowApiSecurityOptions>();
        Builder.Configure<WorkflowTenantCreationOptions>();

        // Basic ASP.NET Core services configuration.
        Builder.Services.AddSingleton(Configuration);
        Builder.Services.AddControllers().AddJsonOptions(ConfigureJson);
        Builder.Services.AddAuthentication(ConfigureAuthentication).AddJwtBearer(ConfigureJwtBearer);
        Builder.Services.AddAuthorization(ConfigureAuthorization);
        Builder.Services.AddEndpointsApiExplorer();
        Builder.Services.AddSwaggerGen(ConfigureSwagger);
        Builder.Services.AddDbContext<DataContext>(ConfigureDbContext);
        
        // Workflow Engine API Data Provider configuration.
        // If not specified explicitly, the first found implementation will be used.
        Builder.Services.AddWorkflowApiMongo();
        Builder.Services.AddWorkflowApiMssql();
        Builder.Services.AddWorkflowApiMysql();
        Builder.Services.AddWorkflowApiOracle();
        Builder.Services.AddWorkflowApiPostgres();
        Builder.Services.AddWorkflowApiSqlite();

        if (!Configuration.MultipleTenantMode)
        {
            // Workflow Engine API quick (single-tenant) setup.
            Builder.Services.AddWorkflowApiCore(ConfigureCore);
            Builder.Services.AddWorkflowApiSecurity(ConfigureSecurity);
            Builder.Services.AddWorkflowRuntime(ConfigureWorkflowEngine);
        }
        else
        {
            // Workflow Engine API multi-tenant setup.
            Builder.Services.AddWorkflowApiCore(options =>
            {
                ConfigureCore(options);
                options.DefaultTenantId = null;
            });

            Builder.Services.AddWorkflowApiSecurity(ConfigureSecurity);

            foreach (var option in Configuration.TenantsConfiguration)
            {
                option.WorkflowRuntimeCreationOptions.ConfigureWorkflowRuntime = ConfigureWorkflowRuntime;
            }

            Builder.Services.AddWorkflowTenants(Configuration.TenantsConfiguration);
        }

        _servicesConfigured = true;
    }

    /// <summary>
    /// Builds the web application with pre-configured middleware.
    /// </summary>
    /// <returns>The web application.</returns>
    public WebApplication Build()
    {
        if (!_servicesConfigured) ConfigureServices();
        
        var app = Builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler(HandleException);
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // Workflow Engine Web API endpoints mapping.
        app.MapWorkflowApi();

        app.MapControllers();
        
        return app;
    }

    private void HandleException(IApplicationBuilder app)
    {
        // Handling and formatting exceptions as JSON.
        app.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;
                
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                
            if (exceptionHandlerPathFeature?.Error is not null)
            {
                await context.Response.WriteAsJsonAsync(Error.Create(exceptionHandlerPathFeature.Error));
            }
        });
    }

    private void ConfigureJson(JsonOptions options)
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    private static void ConfigureAuthentication(AuthenticationOptions options)
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }

    private void ConfigureJwtBearer(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.Jwt.Key)),
            ValidIssuer = Configuration.Jwt.Issuer,
            ValidAudience = Configuration.Jwt.Audience,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
    }

    private static void ConfigureAuthorization(AuthorizationOptions options)
    {
        
    }
    
    private static void ConfigureSwagger(SwaggerGenOptions options)
    {
        // Swagger document configuration.
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Workflow Engine API", 
            Version = "1.0",
            Description = "A Workflow Engine Web API",
        });
        
        // It's necessary to add the security definition to the Swagger document
        // for proper Workflow Engine API specification generation.
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
            BearerFormat = "JWT",
            Name = "Authorization",
            Description = "Please insert JWT Bearer token into field"
        });
        
        options.OperationFilter<SecurityOperationFilter>();
        options.OperationFilter<OperationIdOperationFilter>();
    }

    private static void ConfigureCore(WorkflowApiCoreOptions options)
    {
        
    }
    
    private static void ConfigureSecurity(WorkflowApiSecurityOptions options)
    {
        
    }
    
    private static void ConfigureWorkflowEngine(WorkflowTenantCreationOptions options)
    {
        // Additional Workflow Engine single-tenant configuration can be done here.
        options.WorkflowRuntimeCreationOptions.ConfigureWorkflowRuntime = ConfigureWorkflowRuntime;
    }

    private static void ConfigureWorkflowRuntime(WorkflowRuntime runtime)
    {
        // Additional Workflow Engine runtime configuration can be done here
        // for both single-tenant and multi-tenant modes.
        runtime.AsSingleServer();
    }
    
    private void ConfigureDbContext(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase(Configuration.ConnectionStrings["Default"]);
    }

    private bool _servicesConfigured;
}
