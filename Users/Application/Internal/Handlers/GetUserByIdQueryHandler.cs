using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Model.Queries;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Application.Internal.Handlers;

/// <summary>
/// Handler para la query GetUserByIdQuery
/// </summary>
public class GetUserByIdQueryHandler
{
    private readonly IUserRepository _repository;
    
    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.FindByIdAsync(query.Id, cancellationToken);
    }
}

