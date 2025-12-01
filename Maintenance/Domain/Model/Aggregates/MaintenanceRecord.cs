using System.ComponentModel.DataAnnotations;

namespace movesys_backend_.Maintenance.Domain.Model.Aggregates;

/// <summary>
/// Representa un registro de mantenimiento para un vehículo en MoveSys
/// </summary>
public class MaintenanceRecord
{
    /// <summary>
    /// ID único del registro de mantenimiento (generado automáticamente)
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// ID del vehículo asociado (opcional)
    /// </summary>
    public long? VehicleId { get; set; }
    
    /// <summary>
    /// Placa del vehículo (requerido)
    /// </summary>
    [Required]
    public string VehiclePlate { get; set; } = string.Empty;
    
    /// <summary>
    /// Modelo del vehículo (opcional, información adicional)
    /// </summary>
    public string? Model { get; set; }
    
    /// <summary>
    /// Tipo de mantenimiento: preventive, corrective, emergency (por defecto: "preventive")
    /// </summary>
    public string MaintenanceType { get; set; } = "preventive";
    
    /// <summary>
    /// Descripción del mantenimiento (requerido)
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Costo total del mantenimiento en Soles (requerido)
    /// </summary>
    [Required]
    public decimal Cost { get; set; }
    
    /// <summary>
    /// Kilometraje al momento del mantenimiento
    /// </summary>
    public decimal? Mileage { get; set; }
    
    /// <summary>
    /// Fecha en que se realizó o programó el mantenimiento
    /// </summary>
    public DateTime? MaintenanceDate { get; set; }
    
    /// <summary>
    /// Fecha programada para el próximo mantenimiento
    /// </summary>
    public DateTime? NextMaintenanceDate { get; set; }
    
    /// <summary>
    /// Kilometraje programado para el próximo mantenimiento
    /// </summary>
    public decimal? NextMaintenanceMileage { get; set; }
    
    /// <summary>
    /// Proveedor o taller donde se realizó el mantenimiento (requerido)
    /// </summary>
    [Required]
    public string Provider { get; set; } = string.Empty;
    
    /// <summary>
    /// Lista de repuestos utilizados (JSON serializado)
    /// </summary>
    public string? Parts { get; set; }
    
    /// <summary>
    /// Nombre del mecánico que realizó el mantenimiento
    /// </summary>
    public string? Mechanic { get; set; }
    
    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Estado del mantenimiento: scheduled, in_progress, completed, cancelled (por defecto: "scheduled")
    /// </summary>
    public string Status { get; set; } = "scheduled";
}

