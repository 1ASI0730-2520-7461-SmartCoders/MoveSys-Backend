using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using movesys_backend_.Maintenance.Application.Internal.CommandServices;
using movesys_backend_.Maintenance.Application.Internal.QueryServices;
using movesys_backend_.Maintenance.Domain.Model.Aggregates;

namespace movesys_backend_.Maintenance.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/maintenances")]
public class MaintenanceRecordsController : ControllerBase
{
    private readonly IMaintenanceRecordQueryService _query;
    private readonly IMaintenanceRecordCommandService _cmd;
    
    public MaintenanceRecordsController(IMaintenanceRecordQueryService query, IMaintenanceRecordCommandService cmd)
    {
        _query = query;
        _cmd = cmd;
    }

    /// <summary>
    /// Listar todos los registros de mantenimiento
    /// </summary>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Lista de registros de mantenimiento</returns>
    /// <response code="200">Lista de registros obtenida exitosamente</response>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar todos los registros de mantenimiento")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRecord>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MaintenanceRecord>>> ListAsync(CancellationToken ct)
        => Ok(await _query.ListAsync(ct));

    /// <summary>
    /// Obtener un registro de mantenimiento por su ID
    /// </summary>
    /// <param name="id">ID del registro</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Registro encontrado</returns>
    /// <response code="200">Registro encontrado</response>
    /// <response code="404">Registro no encontrado</response>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Obtener registro de mantenimiento por ID")]
    [ProducesResponseType(typeof(MaintenanceRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaintenanceRecord>> GetById(long id, CancellationToken ct)
    {
        var item = await _query.FindByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Crear un nuevo registro de mantenimiento
    /// </summary>
    /// <remarks>
    /// Ejemplo de solicitud (campos requeridos: vehiclePlate, description, cost, provider):
    /// 
    ///     POST /api/v1/maintenances
    ///     {
    ///         "vehiclePlate": "MJO-234",
    ///         "maintenanceType": "preventive",
    ///         "description": "Cambio de aceite y filtros",
    ///         "cost": 350.50,
    ///         "provider": "Taller Automotriz ABC",
    ///         "maintenanceDate": "2024-11-01T10:00:00Z",
    ///         "mileage": 15000,
    ///         "status": "completed"
    ///     }
    /// 
    /// **Nota:** El campo `id` se genera automáticamente y no debe enviarse. 
    /// El campo `parts` puede enviarse como un string JSON con la lista de repuestos.
    /// </remarks>
    /// <param name="record">Datos del registro de mantenimiento a crear</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Registro creado con su ID generado</returns>
    /// <response code="201">Registro creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Crear un nuevo registro de mantenimiento",
        Description = "Crea un nuevo registro de mantenimiento. Campos requeridos: VehiclePlate, Description, Cost, Provider. Campos opcionales: VehicleId, Model, MaintenanceType (default: 'preventive'), Mileage, MaintenanceDate, NextMaintenanceDate, NextMaintenanceMileage, Parts (JSON string), Mechanic, Notes, Status (default: 'scheduled')"
    )]
    [ProducesResponseType(typeof(MaintenanceRecord), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MaintenanceRecord>> Create([FromBody] MaintenanceRecord record, CancellationToken ct)
    {
        try
        {
            // Validar campos requeridos
            if (record == null)
            {
                return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío" });
            }

            if (string.IsNullOrWhiteSpace(record.VehiclePlate))
            {
                return BadRequest(new { message = "El campo 'vehiclePlate' es requerido" });
            }

            if (string.IsNullOrWhiteSpace(record.Description))
            {
                return BadRequest(new { message = "El campo 'description' es requerido" });
            }

            if (record.Cost < 0)
            {
                return BadRequest(new { message = "El campo 'cost' debe ser mayor o igual a cero" });
            }

            if (string.IsNullOrWhiteSpace(record.Provider))
            {
                return BadRequest(new { message = "El campo 'provider' es requerido" });
            }

            var created = await _cmd.CreateAsync(record, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            // Log del error completo para debugging
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Error al crear el registro de mantenimiento",
                error = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Actualizar un registro de mantenimiento existente
    /// </summary>
    /// <param name="id">ID del registro a actualizar</param>
    /// <param name="record">Datos actualizados del registro</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Sin contenido si la actualización fue exitosa</returns>
    /// <response code="204">Registro actualizado exitosamente</response>
    /// <response code="400">Datos inválidos (campos requeridos faltantes)</response>
    /// <response code="404">Registro no encontrado</response>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Actualizar registro de mantenimiento")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] MaintenanceRecord record, CancellationToken ct)
    {
        // Validar que los campos requeridos estén presentes
        if (record == null)
        {
            return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío" });
        }

        if (string.IsNullOrWhiteSpace(record.VehiclePlate))
        {
            return BadRequest(new { message = "El campo 'vehiclePlate' es requerido" });
        }

        if (string.IsNullOrWhiteSpace(record.Description))
        {
            return BadRequest(new { message = "El campo 'description' es requerido" });
        }

        if (record.Cost < 0)
        {
            return BadRequest(new { message = "El campo 'cost' debe ser mayor o igual a cero" });
        }

        if (string.IsNullOrWhiteSpace(record.Provider))
        {
            return BadRequest(new { message = "El campo 'provider' es requerido" });
        }

        var result = await _cmd.UpdateAsync(id, record, ct);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Eliminar un registro de mantenimiento
    /// </summary>
    /// <param name="id">ID del registro a eliminar</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Sin contenido si la eliminación fue exitosa</returns>
    /// <response code="204">Registro eliminado exitosamente</response>
    /// <response code="404">Registro no encontrado</response>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Eliminar registro de mantenimiento")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _cmd.DeleteAsync(id, ct) ? NoContent() : NotFound();
}

