using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using movesys_backend_.Fleet.Application.Internal.CommandServices;
using movesys_backend_.Fleet.Application.Internal.QueryServices;
using movesys_backend_.Fleet.Domain.Model.Aggregates;

namespace movesys_backend_.Fleet.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleQueryService _queryService;
    private readonly IVehicleCommandService _commandService;

    public VehiclesController(IVehicleQueryService queryService, IVehicleCommandService commandService)
    {
        _queryService = queryService;
        _commandService = commandService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List vehicles")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> ListAsync(CancellationToken ct)
    {
        var items = await _queryService.ListAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get vehicle by id")]
    public async Task<ActionResult<Vehicle>> GetByIdAsync(long id, CancellationToken ct)
    {
        var item = await _queryService.FindByIdAsync(id, ct);
        if (item is null) return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Crear un nuevo vehículo
    /// </summary>
    /// <remarks>
    /// Ejemplo de solicitud (campos requeridos: plate, brand, model, year):
    /// 
    ///     POST /api/v1/vehicles
    ///     {
    ///         "plate": "ABC-123",
    ///         "brand": "Toyota",
    ///         "model": "Hilux",
    ///         "year": 2023,
    ///         "color": "Blanco",
    ///         "type": "truck",
    ///         "capacity": 1500,
    ///         "fuelType": "diesel",
    ///         "status": "available",
    ///         "mileage": 15000
    ///     }
    /// 
    /// **Nota:** El campo `id` se genera automáticamente y no debe enviarse.
    /// </remarks>
    /// <param name="vehicle">Datos del vehículo a crear</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Vehículo creado con su ID generado</returns>
    /// <response code="201">Vehículo creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Crear un nuevo vehículo",
        Description = "Crea un nuevo vehículo. Campos requeridos: Plate, Brand, Model, Year. Campos opcionales: Color, Type (default: 'truck'), Capacity, FuelType (default: 'gasoline'), Status (default: 'available'), CurrentDriver, Mileage (default: 0)"
    )]
    [ProducesResponseType(typeof(Vehicle), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Vehicle>> CreateAsync([FromBody] Vehicle vehicle, CancellationToken ct)
    {
        try
        {
            // Validar campos requeridos
            if (vehicle == null)
            {
                return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío" });
            }

            if (string.IsNullOrWhiteSpace(vehicle.Plate))
            {
                return BadRequest(new { message = "El campo 'plate' es requerido" });
            }

            if (string.IsNullOrWhiteSpace(vehicle.Brand))
            {
                return BadRequest(new { message = "El campo 'brand' es requerido" });
            }

            if (string.IsNullOrWhiteSpace(vehicle.Model))
            {
                return BadRequest(new { message = "El campo 'model' es requerido" });
            }

            if (vehicle.Year <= 0 || vehicle.Year > DateTime.Now.Year + 1)
            {
                return BadRequest(new { message = $"El campo 'year' debe ser un año válido entre 1900 y {DateTime.Now.Year + 1}" });
            }

            var created = await _commandService.CreateAsync(vehicle, ct);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            // Log del error completo para debugging
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Error al crear el vehículo",
                error = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Actualizar un vehículo existente
    /// </summary>
    /// <param name="id">ID del vehículo a actualizar</param>
    /// <param name="vehicle">Datos actualizados del vehículo</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Sin contenido si la actualización fue exitosa</returns>
    /// <response code="204">Vehículo actualizado exitosamente</response>
    /// <response code="404">Vehículo no encontrado</response>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Actualizar vehículo")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] Vehicle vehicle, CancellationToken ct)
    {
        var ok = await _commandService.UpdateAsync(id, vehicle, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Actualización parcial de un vehículo (PATCH)
    /// </summary>
    /// <remarks>
    /// Permite actualizar campos específicos del vehículo sin enviar el objeto completo.
    /// Ejemplos:
    /// 
    /// Asignar conductor:
    ///     PATCH /api/v1/vehicles/1
    ///     {
    ///         "currentDriver": "Juan Pérez",
    ///         "status": "in_use"
    ///     }
    /// 
    /// Cambiar estado:
    ///     PATCH /api/v1/vehicles/1
    ///     {
    ///         "status": "maintenance"
    ///     }
    /// 
    /// Actualizar kilometraje:
    ///     PATCH /api/v1/vehicles/1
    ///     {
    ///         "mileage": 20000
    ///     }
    /// </remarks>
    /// <param name="id">ID del vehículo a actualizar</param>
    /// <param name="patch">Campos a actualizar</param>
    /// <param name="ct">Token de cancelación</param>
    /// <returns>Sin contenido si la actualización fue exitosa</returns>
    /// <response code="204">Vehículo actualizado exitosamente</response>
    /// <response code="404">Vehículo no encontrado</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPatch("{id:long}")]
    [SwaggerOperation(Summary = "Actualización parcial de vehículo (PATCH)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchAsync(long id, [FromBody] Dictionary<string, object> patch, CancellationToken ct)
    {
        var existing = await _queryService.FindByIdAsync(id, ct);
        if (existing is null) return NotFound();

        // Aplicar los cambios del patch
        if (patch.TryGetValue("currentDriver", out var driverObj) || patch.TryGetValue("current_driver", out driverObj))
        {
            existing.CurrentDriver = driverObj?.ToString();
        }

        if (patch.TryGetValue("status", out var statusObj))
        {
            existing.Status = statusObj?.ToString() ?? "available";
        }

        if (patch.TryGetValue("mileage", out var mileageObj))
        {
            if (decimal.TryParse(mileageObj?.ToString(), out var mileage))
            {
                existing.Mileage = mileage;
            }
        }

        if (patch.TryGetValue("plate", out var plateObj))
        {
            existing.Plate = plateObj?.ToString() ?? string.Empty;
        }

        if (patch.TryGetValue("color", out var colorObj))
        {
            existing.Color = colorObj?.ToString();
        }

        var ok = await _commandService.UpdateAsync(id, existing, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete vehicle")]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken ct)
    {
        var ok = await _commandService.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}


