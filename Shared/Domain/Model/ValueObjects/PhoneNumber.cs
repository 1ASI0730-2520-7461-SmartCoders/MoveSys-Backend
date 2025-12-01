using System.Text.RegularExpressions;

namespace movesys_backend_.Shared.Domain.Model.ValueObjects;

/// <summary>
/// Value Object para número de teléfono con validación automática
/// </summary>
public class PhoneNumber
{
    public string Value { get; }
    
    private PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));
        
        // Limpiar el número (remover +51, espacios, guiones)
        var cleaned = value.Replace("+51", "")
                          .Replace(" ", "")
                          .Replace("-", "")
                          .Replace("(", "")
                          .Replace(")", "");
        
        // Formato peruano: 9 dígitos comenzando con 9
        if (!Regex.IsMatch(cleaned, @"^9\d{8}$"))
            throw new ArgumentException($"Invalid phone number format. Expected: 9XXXXXXXX (9 digits starting with 9). Received: {value}", nameof(value));
        
        Value = cleaned;
    }
    
    /// <summary>
    /// Crea una nueva instancia de PhoneNumber con validación
    /// </summary>
    public static PhoneNumber Create(string value) => new(value);
    
    /// <summary>
    /// Intenta crear una instancia, retorna null si es inválido
    /// </summary>
    public static PhoneNumber? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        try
        {
            return new PhoneNumber(value);
        }
        catch
        {
            return null;
        }
    }
    
    public override string ToString() => Value;
    
    public override bool Equals(object? obj)
    {
        return obj is PhoneNumber other && Value == other.Value;
    }
    
    public override int GetHashCode() => Value.GetHashCode();
    
    public static implicit operator string(PhoneNumber phone) => phone.Value;
}

