using Microsoft.EntityFrameworkCore;
using movesys_backend_.Fleet.Domain.Model.Aggregates;
using movesys_backend_.Fleet.Domain.Repositories;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace movesys_backend_.Fleet.Infrastructure.Persistence.EFC.Repositories;

public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Vehicle?> FindByPlateAsync(string plate, CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(v => v.Plate == plate, cancellationToken);
    }
}


