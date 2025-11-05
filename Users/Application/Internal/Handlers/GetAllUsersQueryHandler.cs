using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Model.Queries;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Application.Internal.Handlers;

/// <summary>
/// Handler para la query GetAllUsersQuery
/// </summary>
public class GetAllUsersQueryHandler
{
    private readonly IUserRepository _repository;
    
    public GetAllUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(cancellationToken);
    }
}

