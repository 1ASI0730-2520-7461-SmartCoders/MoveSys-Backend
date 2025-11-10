using movesys_backend_.FuelConsumption.Domain.Model.Aggregates;
using movesys_backend_.FuelConsumption.Domain.Repositories;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.FuelConsumption.Application.Internal.CommandServices;

/// <summary>
/// Command service interface for fuel entry write operations
/// </summary>
public interface IFuelEntryCommandService
{
    Task<FuelEntry> CreateAsync(FuelEntry entry, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, FuelEntry entry, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}

/// <summary>
/// Service for fuel entry command operations (CQRS pattern)
/// </summary>
public class FuelEntryCommandService : IFuelEntryCommandService
{
    private readonly IFuelEntryRepository _repo;
    private readonly IUnitOfWork _uow;
    
    public FuelEntryCommandService(IFuelEntryRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    /// <summary>
    /// Creates a new fuel entry and calculates TotalPaid if not provided
    /// </summary>
    public async Task<FuelEntry> CreateAsync(FuelEntry entry, CancellationToken ct = default)
    {
        // Calculate TotalPaid if not provided
        if (entry.TotalPaid == null || entry.TotalPaid == 0)
        {
            entry.TotalPaid = entry.Liters * entry.CostPerLiter;
        }
        
        await _repo.AddAsync(entry, ct);
        await _uow.CompleteAsync(ct);
        return entry;
    }

    /// <summary>
    /// Updates an existing fuel entry
    /// </summary>
    public async Task<bool> UpdateAsync(long id, FuelEntry entry, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        
        existing.VehicleId = entry.VehicleId;
        existing.VehiclePlate = entry.VehiclePlate;
        existing.Model = entry.Model;
        existing.Liters = entry.Liters;
        existing.CostPerLiter = entry.CostPerLiter;
        
        // Calculate TotalPaid if not provided
        if (entry.TotalPaid == null || entry.TotalPaid == 0)
        {
            existing.TotalPaid = entry.Liters * entry.CostPerLiter;
        }
        else
        {
            existing.TotalPaid = entry.TotalPaid;
        }
        
        existing.FuelType = entry.FuelType;
        existing.Provider = entry.Provider;
        existing.FilledAt = entry.FilledAt;
        existing.Odometer = entry.Odometer;
        existing.Notes = entry.Notes;
        
        _repo.Update(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }

    /// <summary>
    /// Deletes a fuel entry by ID
    /// </summary>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        _repo.Remove(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }
}

