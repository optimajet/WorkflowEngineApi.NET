using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OptimaJet.Workflow.Api;
using WorkflowApi.Data;
using WorkflowApi.Data.Entities;
using WorkflowApi.Swagger;

namespace WorkflowApi.Controllers;

/// <summary>
/// Controller for handling authentication requests and issuing JWT tokens.
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    public AuthController(DataContext data, Configuration configuration, IWorkflowApiPermissions permissions)
    {
        Data = data;
        Configuration = configuration;
        Permissions = permissions;
    }

    /// <summary>
    /// The data context for handling user data.
    /// </summary>
    public DataContext Data { get; }

    /// <summary>
    /// The configuration for the WorkflowApi.
    /// </summary>
    public Configuration Configuration { get; }

    /// <summary>
    /// The permissions service for handling user permissions.
    /// </summary>
    public IWorkflowApiPermissions Permissions { get; }

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    /// <param name="name">The name of the user to log in.</param>
    /// <param name="password">The password of the user to log in.</param>
    /// <returns>The JWT token for the user.</returns>
    [HttpGet]
    [SwaggerOperationId("auth.login")]
    public async Task<ActionResult<string>> Login(string name, string password)
    {
        var user = await Data.Users.FirstOrDefaultAsync(user => user.Name == name && user.Password == password);
        
        if (user == null) return Unauthorized("Invalid username or password");
        
        return await GenerateToken(user);
    }

    private Task<string> GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.Jwt.Key));

        var permissions = JsonSerializer.Deserialize<string[]>(user.Permissions) ?? [];
        var claims = Permissions.AllowList(permissions);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.Union([new Claim(ClaimTypes.Name, user.Name)])),
            Expires = DateTime.UtcNow.AddMilliseconds(Configuration.Jwt.Expires),
            Issuer = Configuration.Jwt.Issuer,
            Audience = Configuration.Jwt.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    /// <summary>
    /// Registers a new user or updates an existing user.
    /// </summary>
    /// <param name="request">The request to register a new user.</param>
    /// <returns>The result of the registration.</returns>
    [HttpPost]
    [SwaggerOperationId("auth.register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var user = new User(request.Name, request.Password, JsonSerializer.Serialize(request.Permissions));

        var existing = await Data.Users.FirstOrDefaultAsync(u => u.Name == request.Name);
        
        if (existing != null)
        {
            existing.Password = request.Password;
            existing.Permissions = JsonSerializer.Serialize(request.Permissions);
            await Data.SaveChangesAsync();
        }
        else
        {
            await Data.Users.AddAsync(user);
            await Data.SaveChangesAsync();
        }
        
        return Ok();
    }

    /// <summary>
    /// Deletes a user by name.
    /// </summary>
    /// <param name="name">The name of the user to delete.</param>
    /// <returns>The result of the deletion.</returns>
    [HttpDelete]
    [SwaggerOperationId("auth.delete")]
    public async Task<ActionResult> Delete(string name)
    {
        var user = await Data.Users.FirstOrDefaultAsync(user => user.Name == name);

        if (user == null) return NotFound();
        
        Data.Users.Remove(user);
        await Data.SaveChangesAsync();
        
        return Ok();
    }

    public record RegisterRequest(string Name, string Password, string[] Permissions);
}
