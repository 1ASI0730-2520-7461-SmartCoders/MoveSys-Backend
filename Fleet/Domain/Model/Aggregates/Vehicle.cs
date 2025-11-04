using System.ComponentModel.DataAnnotations;

namespace movesys_backend_.Fleet.Domain.Model.Aggregates;

/// <summary>
/// Representa un vehículo de la flota MoveSys
/// </summary>
public class Vehicle
{
    /// <summary>
    /// ID único del vehículo (generado automáticamente)
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Placa o matrícula del vehículo (requerido)
    /// </summary>
    [Required]
    public string Plate { get; set; } = string.Empty;
    
    /// <summary>
    /// Marca del vehículo (requerido)
    /// </summary>
    [Required]
    public string Brand { get; set; } = string.Empty;
    
    /// <summary>
    /// Modelo del vehículo (requerido)
    /// </summary>
    [Required]
    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// Año del vehículo (requerido)
    /// </summary>
    [Required]
    public int Year { get; set; }
    
    /// <summary>
    /// Color del vehículo
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Tipo de vehículo: truck, van, car, motorcycle, trailer (por defecto: "truck")
    /// </summary>
    public string Type { get; set; } = "truck";
    
    /// <summary>
    /// Capacidad de carga en kilogramos
    /// </summary>
    public decimal? Capacity { get; set; }
    
    /// <summary>
    /// Tipo de combustible: diesel, gasoline, gas, electric, hybrid (por defecto: "gasoline")
    /// </summary>
    public string FuelType { get; set; } = "gasoline";
    
    /// <summary>
    /// Estado del vehículo: available, in_use, maintenance, out_of_service (por defecto: "available")
    /// </summary>
    public string Status { get; set; } = "available";
    
    /// <summary>
    /// Nombre del conductor actual asignado
    /// </summary>
    public string? CurrentDriver { get; set; }
    
    /// <summary>
    /// Kilometraje actual del vehículo
    /// </summary>
    public decimal Mileage { get; set; } = 0;
}


