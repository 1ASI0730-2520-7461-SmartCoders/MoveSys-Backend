using movesys_backend_.FuelConsumption.Domain.Model.Aggregates;
using movesys_backend_.FuelConsumption.Domain.Repositories;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace movesys_backend_.FuelConsumption.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// EF Core implementation of fuel entry repository
/// </summary>
public class FuelEntryRepository : BaseRepository<FuelEntry>, IFuelEntryRepository
{
    public FuelEntryRepository(AppDbContext context) : base(context)
    {
    }
}

