using movesys_backend_.FuelConsumption.Domain.Model.Aggregates;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.FuelConsumption.Domain.Repositories;

/// <summary>
/// Repository interface for fuel entry persistence operations
/// </summary>
public interface IFuelEntryRepository : IBaseRepository<FuelEntry>
{
}

