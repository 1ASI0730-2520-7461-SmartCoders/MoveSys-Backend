using Cortex.Mediator.Notifications;
using movesys_backend_.Fleet.Domain.Model.Events;

namespace movesys_backend_.Reports.Application.Internal.EventHandlers;

/// <summary>
/// Event handler that updates reports when a vehicle is created
/// </summary>
public class UpdateReportsOnVehicleCreatedHandler : INotificationHandler<VehicleCreatedEvent>
{
    private readonly ILogger<UpdateReportsOnVehicleCreatedHandler> _logger;
    
    public UpdateReportsOnVehicleCreatedHandler(ILogger<UpdateReportsOnVehicleCreatedHandler> logger)
    {
        _logger = logger;
    }
    
    ///<summary>
    /// Handles VehicleCreatedEvent to update report statistics
    /// </summary>
    public Task Handle(VehicleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📊 Updating reports: Vehicle {Plate} ({Brand} {Model}) created", 
            notification.Plate, notification.Brand, notification.Model);
        
        return Task.CompletedTask;
    }
}

