using movesys_backend_.Shared.Domain.Repositories;
using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Application.Internal.CommandServices;

public interface IUserCommandService
{
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, User user, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> UpdateStatusAsync(long id, string status, CancellationToken ct = default);
}

public class UserCommandService : IUserCommandService
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork _uow;
    public UserCommandService(IUserRepository repo, IUnitOfWork uow)
    { _repo = repo; _uow = uow; }

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        await _repo.AddAsync(user, ct);
        await _uow.CompleteAsync(ct);
        return user;
    }

    public async Task<bool> UpdateAsync(long id, User user, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Dni = user.Dni;
        existing.PhoneNumber = user.PhoneNumber;
        existing.Role = user.Role;
        existing.Status = user.Status;
        _repo.Update(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        _repo.Remove(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(long id, string status, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        existing.Status = status;
        _repo.Update(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }
}


