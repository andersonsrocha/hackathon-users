using HackathonUsers.Domain.Models;

namespace HackathonUsers.Security.Interfaces;

public interface IJwtService
{
    public string Generate(User user, IEnumerable<string> roles);
    public string Generate(Guid clientId);
}