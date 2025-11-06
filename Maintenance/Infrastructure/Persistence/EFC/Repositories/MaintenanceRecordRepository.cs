using movesys_backend_.Maintenance.Domain.Model.Aggregates;
using movesys_backend_.Maintenance.Domain.Repositories;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace movesys_backend_.Maintenance.Infrastructure.Persistence.EFC.Repositories;

public class MaintenanceRecordRepository : BaseRepository<MaintenanceRecord>, IMaintenanceRecordRepository
{
    public MaintenanceRecordRepository(AppDbContext context) : base(context)
    {
    }
}

