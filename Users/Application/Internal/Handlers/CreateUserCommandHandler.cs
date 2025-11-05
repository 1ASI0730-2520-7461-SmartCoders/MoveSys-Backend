using movesys_backend_.Shared.Domain.Repositories;
using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Model.Commands;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Application.Internal.Handlers;

/// <summary>
/// Handler para el comando CreateUserCommand
/// </summary>
public class CreateUserCommandHandler
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Dni = command.Dni,
            PhoneNumber = command.PhoneNumber,
            Role = command.Role,
            Status = command.Status
        };
        
        await _repository.AddAsync(user, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);
        
        return user;
    }
}

