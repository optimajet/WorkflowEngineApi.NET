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
using OptimaJet.DataEngine.Sql;
using OptimaJet.Workflow.Api;
using OptimaJet.Workflow.Api.Mongo;
using OptimaJet.Workflow.Api.Mssql;
using OptimaJet.Workflow.Api.Mysql;
using OptimaJet.Workflow.Api.Options;
using OptimaJet.Workflow.Api.Oracle;
using OptimaJet.Workflow.Api.Postgres;
using OptimaJet.Workflow.Api.Sqlite;
using OptimaJet.Workflow.Core.Persistence;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.DbPersistence;
using OptimaJet.Workflow.Migrator;
using OptimaJet.Workflow.MySQL;
using OptimaJet.Workflow.Oracle;
using OptimaJet.Workflow.PostgreSQL;
using OptimaJet.Workflow.SQLite;
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

        Builder.Configuration.AddObject(Configuration);
        Builder.Configure<WorkflowApiCoreOptions>();
        Builder.Configure<WorkflowApiSecurityOptions>();

        Builder.Services.AddSingleton(Configuration);
        Builder.Services.AddControllers().AddJsonOptions(ConfigureJson);
        Builder.Services.AddAuthentication(ConfigureAuthentication).AddJwtBearer(ConfigureJwtBearer);
        Builder.Services.AddAuthorization(ConfigureAuthorization);
        Builder.Services.AddEndpointsApiExplorer();
        Builder.Services.AddSwaggerGen(ConfigureSwagger);
        Builder.Services.AddDbContext<DataContext>(ConfigureDbContext);

        _dataInitializeActions.TryGetValue(Configuration.Provider, out var initializeAction);
        
        if (initializeAction == null)
        {
            throw new NotSupportedException($"The data provider {Configuration.Provider} is not supported.");
        }
        
        initializeAction(this);

        Builder.Services.AddWorkflowApi(ConfigureWorkflowApi);

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
        
        app.UseExceptionHandler(HandleException);
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseWorkflowApi();

        app.MapControllers();
        
        return app;
    }

    private void HandleException(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
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
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Workflow Engine API", 
            Version = "1.0",
            Description = "A Workflow Engine Web API",
        });
        
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Name = "Authorization",
            Description = "Please insert JWT Bearer token into field"
        });
        
        options.OperationFilter<SecurityOperationFilter>();
        options.OperationFilter<OperationIdOperationFilter>();
    }

    private static void ConfigureWorkflowApi(WorkflowApiSetupOptions setupOptions)
    {

    }
    
    private void ConfigureDbContext(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase(Configuration.ConnectionStrings["Default"]);
    }

    private readonly Dictionary<Provider, Action<AppBuilder>> _dataInitializeActions = new()
    {
        {
            Provider.Mongo, builder =>
            {
                var connectionString = builder.Configuration.ConnectionStrings[Provider.Mongo.ToString()];
                builder.Builder.Services.AddWorkflowApiMongo(connectionString);
            }
        },
        {
            Provider.Mssql, builder =>
            {
                var connectionString = builder.Configuration.ConnectionStrings[Provider.Mssql.ToString()];
                builder.Builder.Services.AddWorkflowApiMssql(connectionString, ConfigureWorkflowApiSqlProvider);
                RunMigrations(new MSSQLProvider(connectionString));
            }
        },
        {
            Provider.Mysql, builder =>
            {
                var connectionString = builder.Configuration.ConnectionStrings[Provider.Mysql.ToString()];
                builder.Builder.Services.AddWorkflowApiMysql(connectionString, ConfigureWorkflowApiSqlProvider);
                RunMigrations(new MySQLProvider(connectionString));
            }
        },
        {
            Provider.Oracle, builder =>
            {
                var connectionString = builder.Configuration.ConnectionStrings[Provider.Oracle.ToString()];
                builder.Builder.Services.AddWorkflowApiOracle(connectionString, ConfigureWorkflowApiSqlProvider);
                RunMigrations(new OracleProvider(connectionString));
            }
        },
        {
            Provider.Postgres, builder =>
            {
                var connectionString = builder.Configuration.ConnectionStrings[Provider.Postgres.ToString()];
                builder.Builder.Services.AddWorkflowApiPostgres(connectionString, ConfigureWorkflowApiSqlProvider);
                RunMigrations(new PostgreSQLProvider(connectionString));
            }
        },
        {
            Provider.Sqlite, builder =>
            {
                var connectionString = builder.Configuration.ConnectionStrings[Provider.Sqlite.ToString()];
                builder.Builder.Services.AddWorkflowApiSqlite(connectionString, ConfigureWorkflowApiSqlProvider);
                RunMigrations(new SqliteProvider(connectionString));
            }
        },
    };

    private static void ConfigureWorkflowApiSqlProvider(SqlOptions options)
    {

    }
    
    private static void RunMigrations(IPersistenceProvider provider)
    {
        new WorkflowRuntime().WithPersistenceProvider(provider).RunMigrations();
    }

    private bool _servicesConfigured;
}
