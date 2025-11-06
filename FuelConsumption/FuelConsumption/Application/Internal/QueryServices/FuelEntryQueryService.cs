using movesys_backend_.FuelConsumption.Domain.Model.Aggregates;
using movesys_backend_.FuelConsumption.Domain.Repositories;

namespace movesys_backend_.FuelConsumption.Application.Internal.QueryServices;

/// <summary>
/// Query service interface for fuel entry read operations
/// </summary>
public interface IFuelEntryQueryService
{
    Task<IEnumerable<FuelEntry>> ListAsync(CancellationToken ct = default);
    Task<FuelEntry?> FindByIdAsync(long id, CancellationToken ct = default);
}

/// <summary>
/// Service for fuel entry query operations (CQRS pattern)
/// </summary>
public class FuelEntryQueryService : IFuelEntryQueryService
{
    private readonly IFuelEntryRepository _repo;
    
    public FuelEntryQueryService(IFuelEntryRepository repo)
    {
        _repo = repo;
    }
    
    /// <summary>
    /// Gets all fuel entries
    /// </summary>
    public Task<IEnumerable<FuelEntry>> ListAsync(CancellationToken ct = default) 
        => _repo.ListAsync(ct);
    
    /// <summary>
    /// Gets fuel entry by ID
    /// </summary>
    public Task<FuelEntry?> FindByIdAsync(long id, CancellationToken ct = default) 
        => _repo.FindByIdAsync(id, ct);
}

