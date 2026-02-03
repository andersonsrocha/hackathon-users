using Microsoft.EntityFrameworkCore;
using HackathonUsers.Domain.Interfaces;
using HackathonUsers.Domain.Models;

namespace HackathonUsers.Data.Repositories;

public class UserRepository(HackathonUsersContext usersContext) : IUserRepository
{
    private readonly DbSet<User> _dbSet = usersContext.Set<User>();
    
    public async Task<User?> Find(Guid id, CancellationToken cancellationToken)
        => await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<User>> Find(CancellationToken cancellationToken)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
}