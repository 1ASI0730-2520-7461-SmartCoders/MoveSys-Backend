using movesys_backend_.Shared.Domain.Repositories;
using movesys_backend_.Users.Domain.Model.Commands;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Users.Application.Internal.Handlers;

/// <summary>
/// Handler para el comando UpdateUserStatusCommand
/// </summary>
public class UpdateUserStatusCommandHandler
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateUserStatusCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<bool> Handle(UpdateUserStatusCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.FindByIdAsync(command.Id, cancellationToken);
        if (existing is null) return false;
        
        existing.Status = command.Status;
        _repository.Update(existing);
        await _unitOfWork.CompleteAsync(cancellationToken);
        
        return true;
    }
}

