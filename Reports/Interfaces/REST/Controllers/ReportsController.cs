using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using movesys_backend_.Reports.Application.Internal.QueryServices;

namespace movesys_backend_.Reports.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportsQueryService _queryService;

    public ReportsController(IReportsQueryService queryService)
    {
        _queryService = queryService;
    }

    /// <summary>
    /// Gets unified operations report with all related information
    /// </summary>
    /// <returns>List of unified reports by delivery</returns>
    [HttpGet("unified-operations")]
    [SwaggerOperation(Summary = "Get unified operations report")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetUnifiedOperationsReport(CancellationToken ct)
    {
        var report = await _queryService.GetUnifiedOperationsReportAsync(ct);
        return Ok(report);
    }

    /// <summary>
    /// Gets delivery statistics summary
    /// </summary>
    [HttpGet("deliveries/summary")]
    [SwaggerOperation(Summary = "Get delivery summary")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetDeliverySummary(CancellationToken ct)
    {
        var summary = await _queryService.GetDeliverySummaryAsync(ct);
        return Ok(summary);
    }

    /// <summary>
    /// Gets vehicle statistics summary
    /// </summary>
    [HttpGet("vehicles/summary")]
    [SwaggerOperation(Summary = "Get vehicle summary")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetVehicleSummary(CancellationToken ct)
    {
        var summary = await _queryService.GetVehicleSummaryAsync(ct);
        return Ok(summary);
    }

    /// <summary>
    /// Gets fuel consumption statistics summary
    /// </summary>
    [HttpGet("fuel/summary")]
    [SwaggerOperation(Summary = "Get fuel summary")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetFuelSummary(CancellationToken ct)
    {
        var summary = await _queryService.GetFuelSummaryAsync(ct);
        return Ok(summary);
    }

    /// <summary>
    /// Gets maintenance statistics summary
    /// </summary>
    [HttpGet("maintenance/summary")]
    [SwaggerOperation(Summary = "Get maintenance summary")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetMaintenanceSummary(CancellationToken ct)
    {
        var summary = await _queryService.GetMaintenanceSummaryAsync(ct);
        return Ok(summary);
    }
}

