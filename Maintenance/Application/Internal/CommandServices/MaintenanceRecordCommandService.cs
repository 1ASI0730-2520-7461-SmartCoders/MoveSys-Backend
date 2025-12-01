using movesys_backend_.Maintenance.Domain.Model.Aggregates;
using movesys_backend_.Maintenance.Domain.Repositories;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.Maintenance.Application.Internal.CommandServices;

public interface IMaintenanceRecordCommandService
{
    Task<MaintenanceRecord> CreateAsync(MaintenanceRecord record, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, MaintenanceRecord record, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}

public class MaintenanceRecordCommandService : IMaintenanceRecordCommandService
{
    private readonly IMaintenanceRecordRepository _repo;
    private readonly IUnitOfWork _uow;
    
    public MaintenanceRecordCommandService(IMaintenanceRecordRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<MaintenanceRecord> CreateAsync(MaintenanceRecord record, CancellationToken ct = default)
    {
        await _repo.AddAsync(record, ct);
        await _uow.CompleteAsync(ct);
        return record;
    }

    public async Task<bool> UpdateAsync(long id, MaintenanceRecord record, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        
        existing.VehicleId = record.VehicleId;
        existing.VehiclePlate = record.VehiclePlate;
        existing.Model = record.Model;
        existing.MaintenanceType = record.MaintenanceType;
        existing.Description = record.Description;
        existing.Cost = record.Cost;
        existing.Mileage = record.Mileage;
        existing.MaintenanceDate = record.MaintenanceDate;
        existing.NextMaintenanceDate = record.NextMaintenanceDate;
        existing.NextMaintenanceMileage = record.NextMaintenanceMileage;
        existing.Provider = record.Provider;
        existing.Parts = record.Parts;
        existing.Mechanic = record.Mechanic;
        existing.Notes = record.Notes;
        existing.Status = record.Status;
        
        _repo.Update(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        _repo.Remove(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }
}

