using Microsoft.EntityFrameworkCore;
using movesys_backend_.Fleet.Domain.Model.Aggregates;
using movesys_backend_.Deliveries.Domain.Model.Aggregates;
using movesys_backend_.Conductores.Domain.Model.Aggregates;
using movesys_backend_.FuelConsumption.Domain.Model.Aggregates;
using movesys_backend_.Maintenance.Domain.Model.Aggregates;
using movesys_backend_.IAM.Domain.Model.Aggregates;

namespace movesys_backend_.Shared.Infrastructure.Persistence.EFC;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Conductor> Conductores => Set<Conductor>();
    public DbSet<movesys_backend_.IAM.Domain.Model.Aggregates.User> IamUsers => Set<movesys_backend_.IAM.Domain.Model.Aggregates.User>();
    public DbSet<FuelEntry> FuelEntries => Set<FuelEntry>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // IAM Module - Identity and Access Management
        var iamUserEntity = builder.Entity<movesys_backend_.IAM.Domain.Model.Aggregates.User>();
        iamUserEntity.ToTable("iam_users");
        iamUserEntity.HasKey(u => u.Id);
        iamUserEntity.Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        iamUserEntity.Property(u => u.Username).IsRequired();
        iamUserEntity.Property(u => u.PasswordHash).IsRequired();
        
        // Conductores Module - Drivers Management
        var conductorEntity = builder.Entity<Conductor>();
        conductorEntity.ToTable("drivers");
        conductorEntity.HasKey(c => c.Id);
        conductorEntity.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        conductorEntity.Property(c => c.FirstName).IsRequired();
        conductorEntity.Property(c => c.LastName).IsRequired();
        conductorEntity.Property(c => c.Dni).IsRequired();
        conductorEntity.Property(c => c.PhoneNumber).IsRequired();
        
        // Fleet Module - Vehicle Management
        var vehicleEntity = builder.Entity<Vehicle>();
        vehicleEntity.ToTable("vehicles");
        vehicleEntity.HasKey(v => v.Id);
        vehicleEntity.Property(v => v.Id).IsRequired().ValueGeneratedOnAdd();
        vehicleEntity.Property(v => v.Plate).IsRequired();
        vehicleEntity.Property(v => v.Brand).IsRequired();
        vehicleEntity.Property(v => v.Model).IsRequired();
        vehicleEntity.Property(v => v.Year).IsRequired();
        
        // Deliveries Module - Delivery Management
        var deliveryEntity = builder.Entity<Delivery>();
        deliveryEntity.ToTable("deliveries");
        deliveryEntity.HasKey(d => d.Id);
        deliveryEntity.Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
        deliveryEntity.Property(d => d.Code).IsRequired();
        deliveryEntity.Property(d => d.CustomerName).IsRequired();
        deliveryEntity.Property(d => d.Address).IsRequired();
        deliveryEntity.Property(d => d.OriginProvince).IsRequired();
        deliveryEntity.Property(d => d.DestinationProvince).IsRequired();
        // Configurar DateTime sin precisión de microsegundos para compatibilidad con MySQL 5.7
        deliveryEntity.Property(d => d.ScheduledAt).HasColumnType("datetime");
        
        // FuelConsumption Module - Fuel Tracking
        var fuelEntryEntity = builder.Entity<FuelEntry>();
        fuelEntryEntity.ToTable("fuel_entries");
        fuelEntryEntity.HasKey(f => f.Id);
        fuelEntryEntity.Property(f => f.Id).IsRequired().ValueGeneratedOnAdd();
        fuelEntryEntity.Property(f => f.VehiclePlate).IsRequired();
        fuelEntryEntity.Property(f => f.Liters).IsRequired();
        fuelEntryEntity.Property(f => f.CostPerLiter).IsRequired();
        fuelEntryEntity.Property(f => f.Provider).IsRequired();
        // Configurar DateTime sin precisión de microsegundos
        fuelEntryEntity.Property(f => f.FilledAt).HasColumnType("datetime");
        
        // Maintenance Module - Maintenance Records
        var maintenanceEntity = builder.Entity<MaintenanceRecord>();
        maintenanceEntity.ToTable("maintenance_records");
        maintenanceEntity.HasKey(m => m.Id);
        maintenanceEntity.Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        maintenanceEntity.Property(m => m.VehiclePlate).IsRequired();
        maintenanceEntity.Property(m => m.Description).IsRequired();
        maintenanceEntity.Property(m => m.Cost).IsRequired();
        maintenanceEntity.Property(m => m.Provider).IsRequired();
        // Configurar DateTime sin precisión de microsegundos
        maintenanceEntity.Property(m => m.MaintenanceDate).HasColumnType("datetime");
        maintenanceEntity.Property(m => m.NextMaintenanceDate).HasColumnType("datetime");
        
        // Apply configurations from all assemblies if needed in the future
    }
}

