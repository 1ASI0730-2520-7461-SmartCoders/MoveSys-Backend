using movesys_backend_.Users.Domain.Model.Aggregates;

namespace movesys_backend_.Users.Domain.Model.Queries;

/// <summary>
/// Query para obtener un usuario por ID
/// </summary>
public class GetUserByIdQuery
{
    public long Id { get; set; }
}

