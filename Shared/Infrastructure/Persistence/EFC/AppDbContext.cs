using Microsoft.EntityFrameworkCore;
using movesys_backend_.Fleet.Domain.Model.Aggregates;
using movesys_backend_.Deliveries.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.FuelConsumption.Domain.Model.Aggregates;
using movesys_backend_.Maintenance.Domain.Model.Aggregates;

namespace movesys_backend_.Shared.Infrastructure.Persistence.EFC;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<User> Users => Set<User>();
    public DbSet<FuelEntry> FuelEntries => Set<FuelEntry>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Apply configurations from all assemblies if needed in the future
    }
}

