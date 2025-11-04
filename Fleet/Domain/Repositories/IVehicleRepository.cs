using movesys_backend_.Fleet.Domain.Model.Aggregates;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.Fleet.Domain.Repositories;

public interface IVehicleRepository : IBaseRepository<Vehicle>
{
    Task<Vehicle?> FindByPlateAsync(string plate, CancellationToken cancellationToken = default);
}


