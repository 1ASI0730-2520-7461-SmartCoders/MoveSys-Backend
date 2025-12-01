using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using movesys_backend_.Deliveries.Application.Internal.CommandServices;
using movesys_backend_.Deliveries.Application.Internal.QueryServices;
using movesys_backend_.Deliveries.Domain.Model.Aggregates;

namespace movesys_backend_.Deliveries.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/deliveries")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryQueryService _query;
    private readonly IDeliveryCommandService _cmd;
    public DeliveriesController(IDeliveryQueryService query, IDeliveryCommandService cmd)
    { _query = query; _cmd = cmd; }

    /// <summary>
    /// Listar todas las entregas
    /// </summary>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Lista de entregas</returns>
    /// <response code="200">Lista de entregas obtenida exitosamente</response>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar todas las entregas")]
    [ProducesResponseType(typeof(IEnumerable<Delivery>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Delivery>>> ListAsync(CancellationToken ct)
        => Ok(await _query.ListAsync(ct));

    /// <summary>
    /// Obtener una entrega por su ID
    /// </summary>
    /// <param name="id">ID de la entrega</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Entrega encontrada</returns>
    /// <response code="200">Entrega encontrada</response>
    /// <response code="404">Entrega no encontrada</response>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Obtener entrega por ID")]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Delivery>> GetById(long id, CancellationToken ct)
    {
        var item = await _query.FindByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Crear una nueva entrega
    /// </summary>
    /// <remarks>
    /// Ejemplo de solicitud (campos requeridos: code, customerName, address, originProvince, destinationProvince):
    /// 
    ///     POST /api/v1/deliveries
    ///     {
    ///         "code": "DEL-001",
    ///         "customerName": "Juan Pérez",
    ///         "address": "Av. Principal 123",
    ///         "originProvince": "Lima",
    ///         "destinationProvince": "Arequipa",
    ///         "scheduledAt": "2024-11-01T10:00:00Z",
    ///         "status": "pending",
    ///         "vehiclePlate": "ABC-123",
    ///         "driverName": "Carlos Ramírez",
    ///         "distanceKm": 1000,
    ///         "etaMinutes": 720
    ///     }
    /// 
    /// **Nota:** El campo `id` se genera automáticamente y no debe enviarse.
    /// </remarks>
    /// <param name="delivery">Datos de la entrega a crear</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Entrega creada con su ID generado</returns>
    /// <response code="201">Entrega creada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Crear una nueva entrega",
        Description = "Crea una nueva entrega. Campos requeridos: Code, CustomerName, Address, OriginProvince, DestinationProvince. Campos opcionales: ScheduledAt, Status (default: 'pending'), VehicleId, VehiclePlate, DriverName, EtaMinutes, DistanceKm"
    )]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Delivery>> Create([FromBody] Delivery delivery, CancellationToken ct)
    {
        var created = await _cmd.CreateAsync(delivery, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualizar una entrega existente
    /// </summary>
    /// <param name="id">ID de la entrega a actualizar</param>
    /// <param name="delivery">Datos actualizados de la entrega</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Sin contenido si la actualización fue exitosa</returns>
    /// <response code="204">Entrega actualizada exitosamente</response>
    /// <response code="400">Datos inválidos (campos requeridos faltantes)</response>
    /// <response code="404">Entrega no encontrada</response>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Actualizar entrega")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] Delivery delivery, CancellationToken ct)
    {
        // Validar que los campos requeridos estén presentes
        if (delivery == null)
        {
            return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío" });
        }

        if (string.IsNullOrWhiteSpace(delivery.Code))
        {
            return BadRequest(new { message = "El campo 'code' es requerido" });
        }

        if (string.IsNullOrWhiteSpace(delivery.CustomerName))
        {
            return BadRequest(new { message = "El campo 'customerName' es requerido" });
        }

        if (string.IsNullOrWhiteSpace(delivery.Address))
        {
            return BadRequest(new { message = "El campo 'address' es requerido" });
        }

        if (string.IsNullOrWhiteSpace(delivery.OriginProvince))
        {
            return BadRequest(new { message = "El campo 'originProvince' es requerido" });
        }

        if (string.IsNullOrWhiteSpace(delivery.DestinationProvince))
        {
            return BadRequest(new { message = "El campo 'destinationProvince' es requerido" });
        }

        var result = await _cmd.UpdateAsync(id, delivery, ct);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _cmd.DeleteAsync(id, ct) ? NoContent() : NotFound();
}


