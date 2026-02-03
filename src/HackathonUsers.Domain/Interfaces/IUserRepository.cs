using HackathonUsers.Domain.Models;

namespace HackathonUsers.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> Find(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<User>> Find(CancellationToken cancellationToken);
}