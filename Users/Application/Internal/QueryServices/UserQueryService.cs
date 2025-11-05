using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Application.Internal.QueryServices;

public interface IUserQueryService
{
    Task<IEnumerable<User>> ListAsync(CancellationToken ct = default);
    Task<User?> FindByIdAsync(long id, CancellationToken ct = default);
}

public class UserQueryService : IUserQueryService
{
    private readonly IUserRepository _repo;
    public UserQueryService(IUserRepository repo) { _repo = repo; }

    public Task<IEnumerable<User>> ListAsync(CancellationToken ct = default) => _repo.ListAsync(ct);
    public Task<User?> FindByIdAsync(long id, CancellationToken ct = default) => _repo.FindByIdAsync(id, ct);
}


