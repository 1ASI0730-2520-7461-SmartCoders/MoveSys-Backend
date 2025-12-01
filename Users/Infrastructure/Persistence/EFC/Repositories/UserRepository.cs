using Microsoft.EntityFrameworkCore;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC.Repositories;
using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Infrastructure.Persistence.EFC.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<User?> FindByDniAsync(string dni, CancellationToken ct = default)
        => Entities.AsNoTracking().FirstOrDefaultAsync(u => u.Dni == dni, ct);
}


