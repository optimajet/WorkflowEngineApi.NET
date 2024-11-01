using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OptimaJet.Workflow.Api;
using WorkflowApi.Data.Entities;

namespace WorkflowApi.Data;

/// <summary>
/// Data context for handling user data.
/// </summary>
public sealed class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options, IWorkflowApiPermissions permissions) : base(options)
    {
        Permissions = permissions;
        // Uncomment the following line to recreate the database on next run for migration.
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    /// <summary>
    /// The permissions service for handling user permissions.
    /// </summary>
    public IWorkflowApiPermissions Permissions { get; }

    /// <summary>
    /// The users in the database.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var permissions = JsonSerializer.Serialize(Permissions.GetAllPermissions());
        builder.Entity<User>().HasData(new User("admin", "admin", permissions));
    }
}