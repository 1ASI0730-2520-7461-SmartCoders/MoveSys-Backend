using Microsoft.EntityFrameworkCore;
using movesys_backend_.Shared.Infrastructure.Persistence.EFC;
using movesys_backend_.Deliveries.Domain.Repositories;
using movesys_backend_.Fleet.Domain.Repositories;
using movesys_backend_.FuelConsumption.Domain.Repositories;
using movesys_backend_.Maintenance.Domain.Repositories;
using movesys_backend_.Users.Domain.Repositories;

namespace movesys_backend_.Reports.Application.Internal.QueryServices;

/// <summary>
/// Service for generating unified reports and statistics from multiple bounded contexts
/// </summary>
public class ReportsQueryService : IReportsQueryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IFuelEntryRepository _fuelRepository;
    private readonly IMaintenanceRecordRepository _maintenanceRepository;
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _context;

    public ReportsQueryService(
        IDeliveryRepository deliveryRepository,
        IVehicleRepository vehicleRepository,
        IFuelEntryRepository fuelRepository,
        IMaintenanceRecordRepository maintenanceRepository,
        IUserRepository userRepository,
        AppDbContext context)
    {
        _deliveryRepository = deliveryRepository;
        _vehicleRepository = vehicleRepository;
        _fuelRepository = fuelRepository;
        _maintenanceRepository = maintenanceRepository;
        _userRepository = userRepository;
        _context = context;
    }

    /// <summary>
    /// Aggregates data from deliveries, vehicles, drivers, fuel, and maintenance into unified
    /// </summary>
    public async Task<IEnumerable<object>> GetUnifiedOperationsReportAsync(CancellationToken ct)
    {
        var deliveries = await _deliveryRepository.ListAsync(ct);
        var vehicles = await _vehicleRepository.ListAsync(ct);
        var fuelEntries = await _fuelRepository.ListAsync(ct);
        var maintenanceRecords = await _maintenanceRepository.ListAsync(ct);
        var users = await _userRepository.ListAsync(ct);

        var report = deliveries.Select(delivery =>
        {
            var vehiclePlate = delivery.VehiclePlate ?? "";
            var vehicle = vehicles.FirstOrDefault(v => 
                (v.Plate ?? "") == vehiclePlate
            );

            var driverName = delivery.DriverName ?? vehicle?.CurrentDriver ?? "N/A";
            var driverInfo = users.FirstOrDefault(u => 
                $"{u.FirstName} {u.LastName}" == driverName
            ) ?? users.FirstOrDefault(u => u.FirstName == driverName || u.LastName == driverName);

            var fuelData = fuelEntries.Where(e => 
                (e.VehiclePlate ?? "") == vehiclePlate
            ).ToList();
            var totalFuelLiters = fuelData.Sum(e => e.Liters);
            var totalFuelCost = fuelData.Sum(e => e.TotalPaid);

            var maintenanceData = maintenanceRecords.Where(m => 
                (m.VehiclePlate ?? "") == vehiclePlate
            ).ToList();
            var totalMaintenanceCost = maintenanceData.Sum(m => m.Cost);

            return new
            {
                // Delivery info
                code = delivery.Code ?? "N/A",
                customerName = delivery.CustomerName ?? "N/A",
                originProvince = delivery.OriginProvince ?? "N/A",
                destinationProvince = delivery.DestinationProvince ?? "N/A",
                distanceKm = delivery.DistanceKm ?? 0,
                status = delivery.Status ?? "pending",

                // Vehicle info
                vehiclePlate = vehiclePlate != "" ? vehiclePlate : "N/A",
                vehicleModel = vehicle != null ? vehicle.Model : "N/A",
                vehicleMileage = vehicle?.Mileage ?? 0m,
                vehicleStatus = vehicle?.Status ?? "N/A",

                // Driver info
                driverName = driverName,
                driverDni = driverInfo?.Dni ?? "N/A",
                driverPhone = driverInfo?.PhoneNumber ?? "N/A",

                // Fuel info
                totalFuelLiters = totalFuelLiters,
                totalFuelCost = totalFuelCost,
                fuelEntriesCount = fuelData.Count,

                // Maintenance info
                totalMaintenanceCost = totalMaintenanceCost,
                maintenanceCount = maintenanceData.Count
            };
        }).ToList();

        return report;
    }

    /// <summary>
    /// Calculates delivery statistics: total, pending, in progress, completed, cancelled, total
    /// </summary>
    public async Task<object> GetDeliverySummaryAsync(CancellationToken ct)
    {
        var deliveries = await _deliveryRepository.ListAsync(ct);
        
        return new
        {
            total = deliveries.Count(),
            pending = deliveries.Count(d => d.Status == "pending"),
            inProgress = deliveries.Count(d => d.Status == "in_progress" || d.Status == "in_transit"),
            completed = deliveries.Count(d => d.Status == "completed"),
            cancelled = deliveries.Count(d => d.Status == "cancelled"),
            totalDistance = deliveries.Sum(d => d.DistanceKm ?? 0)
        };
    }

    /// <summary>
    /// Calculates vehicle statistics: total, available, in use, maintenance, out of service, total mileage
    /// </summary>
    public async Task<object> GetVehicleSummaryAsync(CancellationToken ct)
    {
        var vehicles = await _vehicleRepository.ListAsync(ct);
        
        return new
        {
            total = vehicles.Count(),
            available = vehicles.Count(v => v.Status == "available"),
            inUse = vehicles.Count(v => v.Status == "in_use"),
            maintenance = vehicles.Count(v => v.Status == "maintenance"),
            outOfService = vehicles.Count(v => v.Status == "out_of_service"),
            totalMileage = vehicles.Sum(v => v.Mileage)
        };
    }

    /// <summary>
    /// Calculates fuel consumption statistics: total entries, liters, cost, average cost per liter
    /// </summary>
    public async Task<object> GetFuelSummaryAsync(CancellationToken ct)
    {
        var fuelEntries = await _fuelRepository.ListAsync(ct);
        
        return new
        {
            totalEntries = fuelEntries.Count(),
            totalLiters = fuelEntries.Sum(e => e.Liters),
            totalCost = fuelEntries.Sum(e => e.TotalPaid),
            averageCostPerLiter = fuelEntries.Any() ? fuelEntries.Average(e => e.CostPerLiter) : 0
        };
    }

    /// <summary>
    /// Calculates maintenance statistics: total, scheduled, in progress, completed, total cost
    /// </summary>
    public async Task<object> GetMaintenanceSummaryAsync(CancellationToken ct)
    {
        var maintenanceRecords = await _maintenanceRepository.ListAsync(ct);
        
        return new
        {
            total = maintenanceRecords.Count(),
            scheduled = maintenanceRecords.Count(m => m.Status == "scheduled"),
            inProgress = maintenanceRecords.Count(m => m.Status == "in_progress"),
            completed = maintenanceRecords.Count(m => m.Status == "completed"),
            totalCost = maintenanceRecords.Sum(m => m.Cost)
        };
    }
}

