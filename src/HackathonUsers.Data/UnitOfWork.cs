using HackathonUsers.Domain.Interfaces;

namespace HackathonUsers.Data;

public sealed class UnitOfWork(HackathonUsersContext usersContext) : IUnitOfWork
{
    public async Task<bool> CommitAsync(CancellationToken cancellationToken)
        => await usersContext.SaveChangesAsync(cancellationToken) > 0;
}