using movesys_backend_.Fleet.Domain.Model.Aggregates;

namespace movesys_backend_.Fleet.Application.Internal.QueryServices;

public interface IVehicleQueryService
{
    Task<IEnumerable<Vehicle>> ListAsync(CancellationToken cancellationToken = default);
    Task<Vehicle?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
}


