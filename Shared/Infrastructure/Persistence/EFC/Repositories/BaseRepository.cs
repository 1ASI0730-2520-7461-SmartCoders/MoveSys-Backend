using Microsoft.EntityFrameworkCore;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.Shared.Infrastructure.Persistence.EFC.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> Entities;

    public BaseRepository(AppDbContext context)
    {
        Context = context;
        Entities = context.Set<T>();
    }

    public async Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default)
        => await Entities.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<T?> FindByIdAsync(long id, CancellationToken cancellationToken = default)
        => await Entities.FindAsync([id], cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await Entities.AddAsync(entity, cancellationToken);

    public void Update(T entity) => Entities.Update(entity);

    public void Remove(T entity) => Entities.Remove(entity);
}


