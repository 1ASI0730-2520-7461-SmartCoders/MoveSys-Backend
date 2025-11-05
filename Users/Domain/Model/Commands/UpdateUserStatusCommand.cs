namespace movesys_backend_.Users.Domain.Model.Commands;

/// <summary>
/// Command para actualizar el estado de un usuario
/// </summary>
public class UpdateUserStatusCommand
{
    public long Id { get; set; }
    public string Status { get; set; } = "active";
}

