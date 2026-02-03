using Microsoft.AspNetCore.Identity;

namespace HackathonUsers.Domain.Models;

public sealed class User : IdentityUser<Guid>
{
    public User() {}
    
    public User(string name, string email) : base(email)
    {
        Name = name;
        Email = email;
        NormalizedUserName = email.ToUpper();
        NormalizedEmail = email.ToUpper();
    }
    
    public string Name { get; init; } = string.Empty;
}