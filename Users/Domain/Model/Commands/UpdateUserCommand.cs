namespace movesys_backend_.Users.Domain.Model.Commands;

/// <summary>
/// Command para actualizar un usuario
/// </summary>
public class UpdateUserCommand
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "driver";
    public string Status { get; set; } = "active";
}

