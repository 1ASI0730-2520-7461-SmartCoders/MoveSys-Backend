using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.Shared.Infrastructure.Persistence.EFC.Configuration;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}



