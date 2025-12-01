using System.ComponentModel.DataAnnotations;

namespace movesys_backend_.Users.Domain.Model.Aggregates;

/// <summary>
/// Representa un usuario del sistema MoveSys
/// </summary>
public class User
{
    /// <summary>
    /// ID único del usuario (generado automáticamente por el backend)
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Primer nombre del usuario (requerido)
    /// </summary>
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Apellido del usuario (requerido)
    /// </summary>
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Documento Nacional de Identidad (requerido)
    /// </summary>
    [Required]
    public string Dni { get; set; } = string.Empty;
    
    /// <summary>
    /// Número de teléfono del usuario (requerido)
    /// </summary>
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Rol del usuario (por defecto: "driver")
    /// </summary>
    public string Role { get; set; } = "driver";
    
    /// <summary>
    /// Estado del usuario (por defecto: "active")
    /// </summary>
    public string Status { get; set; } = "active";
}


