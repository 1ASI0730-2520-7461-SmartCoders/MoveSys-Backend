using movesys_backend_.Shared.Domain.Repositories;
using movesys_backend_.Users.Domain.Model.Commands;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Application.Internal.Handlers;

/// <summary>
/// Handler para el comando DeleteUserCommand
/// </summary>
public class DeleteUserCommandHandler
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.FindByIdAsync(command.Id, cancellationToken);
        if (existing is null) return false;
        
        _repository.Remove(existing);
        await _unitOfWork.CompleteAsync(cancellationToken);
        
        return true;
    }
}

