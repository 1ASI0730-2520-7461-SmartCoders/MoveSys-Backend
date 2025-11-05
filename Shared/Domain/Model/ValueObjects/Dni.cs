using System.Text.RegularExpressions;

namespace movesys_backend_.Shared.Domain.Model.ValueObjects;

/// <summary>
/// Value Object para DNI (Documento Nacional de Identidad) con validación automática
/// </summary>
public class Dni
{
    public string Value { get; }
    
    private Dni(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("DNI cannot be empty", nameof(value));
        
        // DNI peruano: exactamente 8 dígitos
        if (!Regex.IsMatch(value, @"^\d{8}$"))
            throw new ArgumentException($"Invalid DNI format. Expected: 8 digits. Received: {value}", nameof(value));
        
        Value = value;
    }
    
    /// <summary>
    /// Crea una nueva instancia de Dni con validación
    /// </summary>
    public static Dni Create(string value) => new(value);
    
    /// <summary>
    /// Intenta crear una instancia, retorna null si es inválido
    /// </summary>
    public static Dni? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        try
        {
            return new Dni(value);
        }
        catch
        {
            return null;
        }
    }
    
    public override string ToString() => Value;
    
    public override bool Equals(object? obj)
    {
        return obj is Dni other && Value == other.Value;
    }
    
    public override int GetHashCode() => Value.GetHashCode();
    
    public static implicit operator string(Dni dni) => dni.Value;
}

