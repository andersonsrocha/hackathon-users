using HackathonUsers.Domain.Models;

namespace HackathonUsers.Security;

public interface IJwtService
{
    public string Generate(User user, IEnumerable<string> roles);
}