namespace movesys_backend_.Shared.Domain.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}


