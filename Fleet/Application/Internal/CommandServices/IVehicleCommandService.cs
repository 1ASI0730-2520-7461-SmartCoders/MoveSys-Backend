using movesys_backend_.Fleet.Domain.Model.Aggregates;

namespace movesys_backend_.Fleet.Application.Internal.CommandServices;

public interface IVehicleCommandService
{
    Task<Vehicle> CreateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(long id, Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}


