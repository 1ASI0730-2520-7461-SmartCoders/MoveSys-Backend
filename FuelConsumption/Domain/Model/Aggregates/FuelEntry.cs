using System.ComponentModel.DataAnnotations;

namespace movesys_backend_.FuelConsumption.Domain.Model.Aggregates;

/// <summary>
/// Fuel consumption entry record in MoveSys
/// </summary>
public class FuelEntry
{
    /// <summary>
    /// Unique record ID (auto-generated)
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Vehicle ID (optional, nullable if only plate is available)
    /// </summary>
    public long? VehicleId { get; set; }
    
    /// <summary>
    /// Vehicle plate (required)
    /// </summary>
    [Required]
    public string VehiclePlate { get; set; } = string.Empty;
    
    /// <summary>
    /// Vehicle model (optional, additional info)
    /// </summary>
    public string? Model { get; set; }
    
    /// <summary>
    /// Fuel liters loaded (required)
    /// </summary>
    [Required]
    public decimal Liters { get; set; }
    
    /// <summary>
    /// Cost per liter in Soles (required)
    /// </summary>
    [Required]
    public decimal CostPerLiter { get; set; }
    
    /// <summary>
    /// Total paid in Soles (optional, calculated as Liters * CostPerLiter if not provided)
    /// </summary>
    public decimal? TotalPaid { get; set; }
    
    /// <summary>
    /// Fuel type: diesel, gasoline, gas, electric (default: "diesel")
    /// </summary>
    public string FuelType { get; set; } = "diesel";
    
    /// <summary>
    /// Provider or gas station name (required)
    /// </summary>
    [Required]
    public string Provider { get; set; } = string.Empty;
    
    /// <summary>
    /// Fuel fill date and time
    /// </summary>
    public DateTime? FilledAt { get; set; }
    
    /// <summary>
    /// Odometer reading at fill time
    /// </summary>
    public decimal? Odometer { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
}

