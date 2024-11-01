using Microsoft.EntityFrameworkCore;

namespace WorkflowApi.Data.Entities;

/// <summary>
/// Represents a user in the database.
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class User
{
    public User(string name, string password, string permissions)
    {
        Name = name;
        Password = password;
        Permissions = permissions;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Password { get; set; }
    public string Permissions { get; set; }
}