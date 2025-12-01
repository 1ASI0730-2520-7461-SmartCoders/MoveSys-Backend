using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using movesys_backend_.Users.Domain.Model.Aggregates;
using movesys_backend_.Users.Domain.Model.Commands;
using movesys_backend_.Users.Domain.Model.Queries;
using movesys_backend_.Users.Application.Internal.Handlers;

namespace movesys_backend_.Users.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly CreateUserCommandHandler _createHandler;
    private readonly UpdateUserCommandHandler _updateHandler;
    private readonly DeleteUserCommandHandler _deleteHandler;
    private readonly UpdateUserStatusCommandHandler _updateStatusHandler;
    private readonly GetAllUsersQueryHandler _getAllHandler;
    private readonly GetUserByIdQueryHandler _getByIdHandler;
    
    public UsersController(
        CreateUserCommandHandler createHandler,
        UpdateUserCommandHandler updateHandler,
        DeleteUserCommandHandler deleteHandler,
        UpdateUserStatusCommandHandler updateStatusHandler,
        GetAllUsersQueryHandler getAllHandler,
        GetUserByIdQueryHandler getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _updateStatusHandler = updateStatusHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List users")]
    public async Task<ActionResult<IEnumerable<User>>> ListAsync(CancellationToken ct)
    {
        var query = new GetAllUsersQuery();
        var users = await _getAllHandler.Handle(query, ct);
        return Ok(users);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<User>> GetById(long id, CancellationToken ct)
    {
        var query = new GetUserByIdQuery { Id = id };
        var item = await _getByIdHandler.Handle(query, ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Crear un nuevo usuario
    /// </summary>
    /// <remarks>
    /// Ejemplo de solicitud (acepta PascalCase o camelCase):
    /// 
    ///     POST /api/v1/users
    ///     {
    ///         "firstName": "Juan",
    ///         "lastName": "Pérez",
    ///         "dni": "12345678",
    ///         "phoneNumber": "987654321",
    ///         "role": "driver",
    ///         "status": "active"
    ///     }
    /// 
    /// **Nota:** El campo `id` se genera automáticamente y no debe enviarse.
    /// 
    /// **Respuesta:** El backend devuelve el usuario creado en formato camelCase con el código 201 Created. Solo incluye los campos: id, firstName, lastName, dni, phoneNumber, role, status.
    /// </remarks>
    /// <param name="user">Datos del usuario a crear</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Usuario creado con su ID generado</returns>
    /// <response code="201">Usuario creado exitosamente. Devuelve el objeto usuario en formato camelCase.</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Crear un nuevo usuario",
        Description = "Crea un nuevo usuario. Campos requeridos: FirstName/firstName, LastName/lastName, Dni/dni, PhoneNumber/phoneNumber. Campos opcionales: Role/role (default: 'driver'), Status/status (default: 'active'). La respuesta se devuelve en formato camelCase."
    )]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> Create([FromBody] User user, CancellationToken ct)
    {
        var command = new CreateUserCommand
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Dni = user.Dni,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Status = user.Status
        };
        
        var created = await _createHandler.Handle(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] User user, CancellationToken ct)
    {
        var command = new UpdateUserCommand
        {
            Id = id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Dni = user.Dni,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Status = user.Status
        };
        
        var result = await _updateHandler.Handle(command, ct);
        return result ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Patch(long id, [FromBody] Dictionary<string, object> patch, CancellationToken ct)
    {
        if (patch.TryGetValue("status", out var statusObj))
        {
            var status = statusObj?.ToString() ?? "active";
            var command = new UpdateUserStatusCommand { Id = id, Status = status };
            var result = await _updateStatusHandler.Handle(command, ct);
            return result ? NoContent() : NotFound();
        }
        return BadRequest("Unsupported patch");
    }

    [HttpPatch("reset-password")]
    public IActionResult ResetPassword([FromBody] Dictionary<string, string> body)
    {
        var email = body.GetValueOrDefault("email");
        if (string.IsNullOrWhiteSpace(email)) return BadRequest();
        return Ok(new { message = "Reset password email sent (mock)", email });
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var command = new DeleteUserCommand { Id = id };
        var result = await _deleteHandler.Handle(command, ct);
        return result ? NoContent() : NotFound();
    }
}


