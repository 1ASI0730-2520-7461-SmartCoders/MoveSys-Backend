using movesys_backend_.Deliveries.Domain.Model.Aggregates;
using movesys_backend_.Deliveries.Domain.Repositories;

namespace movesys_backend_.Deliveries.Application.Internal.QueryServices;

public interface IDeliveryQueryService
{
    Task<IEnumerable<Delivery>> ListAsync(CancellationToken ct = default);
    Task<Delivery?> FindByIdAsync(long id, CancellationToken ct = default);
}

public class DeliveryQueryService : IDeliveryQueryService
{
    private readonly IDeliveryRepository _repo;
    public DeliveryQueryService(IDeliveryRepository repo)
    {
        _repo = repo;
    }
    public Task<IEnumerable<Delivery>> ListAsync(CancellationToken ct = default) => _repo.ListAsync(ct);
    public Task<Delivery?> FindByIdAsync(long id, CancellationToken ct = default) => _repo.FindByIdAsync(id, ct);
}