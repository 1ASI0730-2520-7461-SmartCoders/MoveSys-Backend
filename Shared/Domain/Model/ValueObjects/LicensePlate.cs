using System.Text.RegularExpressions;

namespace movesys_backend_.Shared.Domain.Model.ValueObjects;

/// <summary>
/// Value Object para placa de vehículo con validación automática
/// </summary>
public class LicensePlate
{
    public string Value { get; }
    
    private LicensePlate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("License plate cannot be empty", nameof(value));
        
        // Formato peruano: ABC-1234 o ABC-123
        var normalized = value.Trim().ToUpper();
        if (!Regex.IsMatch(normalized, @"^[A-Z]{3}-\d{3,4}$"))
            throw new ArgumentException($"Invalid license plate format. Expected format: ABC-123 or ABC-1234. Received: {value}", nameof(value));
        
        Value = normalized;
    }
    
    /// <summary>
    /// Crea una nueva instancia de LicensePlate con validación
    /// </summary>
    public static LicensePlate Create(string value) => new(value);
    
    /// <summary>
    /// Intenta crear una instancia, retorna null si es inválido
    /// </summary>
    public static LicensePlate? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        try
        {
            return new LicensePlate(value);
        }
        catch
        {
            return null;
        }
    }
    
    public override string ToString() => Value;
    
    public override bool Equals(object? obj)
    {
        return obj is LicensePlate other && Value == other.Value;
    }
    
    public override int GetHashCode() => Value.GetHashCode();
    
    public static implicit operator string(LicensePlate plate) => plate.Value;
}

