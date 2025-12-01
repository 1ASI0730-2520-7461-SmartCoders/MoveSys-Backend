using movesys_backend_.Shared.Domain.Repositories;
using movesys_backend_.Users.Domain.Model.Aggregates;

namespace movesys_backend_.Users.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByDniAsync(string dni, CancellationToken ct = default);
}


