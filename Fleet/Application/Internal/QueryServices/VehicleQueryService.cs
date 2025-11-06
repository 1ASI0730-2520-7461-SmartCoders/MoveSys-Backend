using movesys_backend_.Fleet.Domain.Model.Aggregates;
using movesys_backend_.Fleet.Domain.Repositories;

namespace movesys_backend_.Fleet.Application.Internal.QueryServices;

public class VehicleQueryService : IVehicleQueryService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleQueryService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public Task<IEnumerable<Vehicle>> ListAsync(CancellationToken cancellationToken = default)
        => _vehicleRepository.ListAsync(cancellationToken);

    public Task<Vehicle?> FindByIdAsync(long id, CancellationToken cancellationToken = default)
        => _vehicleRepository.FindByIdAsync(id, cancellationToken);
}


