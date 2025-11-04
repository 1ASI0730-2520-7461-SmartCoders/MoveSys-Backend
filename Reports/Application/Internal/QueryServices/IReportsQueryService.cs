namespace movesys_backend_.Reports.Application.Internal.QueryServices;

/// <summary>
/// Query service for generating reports and statistics
/// </summary>
public interface IReportsQueryService
{
    ///<summary>
    /// Gets unified report combining deliveries, vehicles, drivers, fuel, and maintenance data
    /// </summary>
    Task<IEnumerable<object>> GetUnifiedOperationsReportAsync(CancellationToken ct);
    
    /// <summary>
    /// Gets delivery statistics summary
    /// </summary>
    Task<object> GetDeliverySummaryAsync(CancellationToken ct);
    
    /// <summary>
    /// Gets vehicle statistics summary
    /// </summary>
    Task<object> GetVehicleSummaryAsync(CancellationToken ct);
    
    /// <summary>
    /// Gets fuel consumption statistics summary
    /// </summary>
    Task<object> GetFuelSummaryAsync(CancellationToken ct);
    
    /// <summary>
    /// Gets maintenance statistics summary
    /// </summary>
    Task<object> GetMaintenanceSummaryAsync(CancellationToken ct);
}

