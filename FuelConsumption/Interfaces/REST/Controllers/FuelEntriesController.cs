using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using movesys_backend_.FuelConsumption.Application.Internal.CommandServices;
using movesys_backend_.FuelConsumption.Application.Internal.QueryServices;
using movesys_backend_.FuelConsumption.Domain.Model.Aggregates;

namespace movesys_backend_.FuelConsumption.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/fuel-entries")]
public class FuelEntriesController : ControllerBase
{
    private readonly IFuelEntryQueryService _query;
    private readonly IFuelEntryCommandService _cmd;
    
    public FuelEntriesController(IFuelEntryQueryService query, IFuelEntryCommandService cmd)
    {
        _query = query;
        _cmd = cmd;
    }

    /// <summary>
    /// Lists all fuel entries
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List all fuel entries")]
    [ProducesResponseType(typeof(IEnumerable<FuelEntry>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FuelEntry>>> ListAsync(CancellationToken ct)
        => Ok(await _query.ListAsync(ct));

    /// <summary>
    /// Gets fuel entry by ID
    /// </summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get fuel entry by ID")]
    [ProducesResponseType(typeof(FuelEntry), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FuelEntry>> GetById(long id, CancellationToken ct)
    {
        var item = await _query.FindByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Creates a new fuel entry. Required: vehiclePlate, liters, costPerLiter, provider
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create fuel entry")]
    [ProducesResponseType(typeof(FuelEntry), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FuelEntry>> Create([FromBody] FuelEntry entry, CancellationToken ct)
    {
        try
        {
            // Validate required fields
            if (entry == null)
            {
                return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío" });
            }

            if (string.IsNullOrWhiteSpace(entry.VehiclePlate))
            {
                return BadRequest(new { message = "El campo 'vehiclePlate' es requerido" });
            }

            if (entry.Liters <= 0)
            {
                return BadRequest(new { message = "El campo 'liters' debe ser mayor a cero" });
            }

            if (entry.CostPerLiter <= 0)
            {
                return BadRequest(new { message = "El campo 'costPerLiter' debe ser mayor a cero" });
            }

            if (string.IsNullOrWhiteSpace(entry.Provider))
            {
                return BadRequest(new { message = "El campo 'provider' es requerido" });
            }

            var created = await _cmd.CreateAsync(entry, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            // Log full error for debugging
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Error al crear el registro de combustible",
                error = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Updates an existing fuel entry
    /// </summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update fuel entry")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] FuelEntry entry, CancellationToken ct)
    {
        // Validate required fields
        if (entry == null)
        {
            return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vacío" });
        }

        if (string.IsNullOrWhiteSpace(entry.VehiclePlate))
        {
            return BadRequest(new { message = "El campo 'vehiclePlate' es requerido" });
        }

        if (entry.Liters <= 0)
        {
            return BadRequest(new { message = "El campo 'liters' debe ser mayor a cero" });
        }

        if (entry.CostPerLiter <= 0)
        {
            return BadRequest(new { message = "El campo 'costPerLiter' debe ser mayor a cero" });
        }

        if (string.IsNullOrWhiteSpace(entry.Provider))
        {
            return BadRequest(new { message = "El campo 'provider' es requerido" });
        }

        var result = await _cmd.UpdateAsync(id, entry, ct);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a fuel entry
    /// </summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete fuel entry")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _cmd.DeleteAsync(id, ct) ? NoContent() : NotFound();
}

