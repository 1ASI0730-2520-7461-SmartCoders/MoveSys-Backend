using System.ComponentModel.DataAnnotations;

namespace movesys_backend_.Deliveries.Domain.Model.Aggregates;

/// <summary>
/// Representa una entrega del sistema MoveSys
/// </summary>
public class Delivery
{
    /// <summary>
    /// ID único de la entrega (generado automáticamente)
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Código único de la entrega (requerido)
    /// </summary>
    [Required]
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Nombre del cliente (requerido)
    /// </summary>
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Dirección de entrega (requerido)
    /// </summary>
    [Required]
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Provincia de origen (requerido)
    /// </summary>
    [Required]
    public string OriginProvince { get; set; } = string.Empty;
    
    /// <summary>
    /// Provincia de destino (requerido)
    /// </summary>
    [Required]
    public string DestinationProvince { get; set; } = string.Empty;
    
    /// <summary>
    /// Fecha y hora programada para la entrega
    /// </summary>
    public DateTime? ScheduledAt { get; set; }
    
    /// <summary>
    /// Estado de la entrega: pending, in_progress, completed, cancelled (por defecto: "pending")
    /// </summary>
    public string Status { get; set; } = "pending";
    
    /// <summary>
    /// ID del vehículo asignado (opcional)
    /// </summary>
    public long? VehicleId { get; set; }
    
    /// <summary>
    /// Placa del vehículo asignado (opcional)
    /// </summary>
    public string? VehiclePlate { get; set; }
    
    /// <summary>
    /// Nombre del conductor asignado (opcional)
    /// </summary>
    public string? DriverName { get; set; }
    
    /// <summary>
    /// Tiempo estimado de llegada en minutos (opcional)
    /// </summary>
    public int? EtaMinutes { get; set; }
    
    /// <summary>
    /// Distancia en kilómetros (opcional)
    /// </summary>
    public double? DistanceKm { get; set; }
}


