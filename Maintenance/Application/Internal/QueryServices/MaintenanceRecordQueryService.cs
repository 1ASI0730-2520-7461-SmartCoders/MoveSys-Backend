using movesys_backend_.Maintenance.Domain.Model.Aggregates;
using movesys_backend_.Maintenance.Domain.Repositories;

namespace movesys_backend_.Maintenance.Application.Internal.QueryServices;

public interface IMaintenanceRecordQueryService
{
    Task<IEnumerable<MaintenanceRecord>> ListAsync(CancellationToken ct = default);
    Task<MaintenanceRecord?> FindByIdAsync(long id, CancellationToken ct = default);
}

public class MaintenanceRecordQueryService : IMaintenanceRecordQueryService
{
    private readonly IMaintenanceRecordRepository _repo;
    
    public MaintenanceRecordQueryService(IMaintenanceRecordRepository repo)
    {
        _repo = repo;
    }
    
    public Task<IEnumerable<MaintenanceRecord>> ListAsync(CancellationToken ct = default) 
        => _repo.ListAsync(ct);
    
    public Task<MaintenanceRecord?> FindByIdAsync(long id, CancellationToken ct = default) 
        => _repo.FindByIdAsync(id, ct);
}

