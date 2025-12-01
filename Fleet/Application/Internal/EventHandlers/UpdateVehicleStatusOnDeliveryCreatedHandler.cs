using Cortex.Mediator.Notifications;
using movesys_backend_.Deliveries.Domain.Model.Events;
using movesys_backend_.Fleet.Domain.Repositories;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.Fleet.Application.Internal.EventHandlers;

/// <summary>
/// Handler que actualiza el estado del vehículo cuando se crea una entrega
/// </summary>
public class UpdateVehicleStatusOnDeliveryCreatedHandler : INotificationHandler<DeliveryCreatedEvent>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateVehicleStatusOnDeliveryCreatedHandler> _logger;
    
    public UpdateVehicleStatusOnDeliveryCreatedHandler(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateVehicleStatusOnDeliveryCreatedHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task Handle(DeliveryCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.VehiclePlate))
        {
            return; // No hay vehículo asignado
        }
        
        var vehicle = (await _vehicleRepository.ListAsync(cancellationToken))
            .FirstOrDefault(v => v.Plate == notification.VehiclePlate);
        
        if (vehicle != null && vehicle.Status == "available")
        {
            vehicle.Status = "in_use";
            _vehicleRepository.Update(vehicle);
            await _unitOfWork.CompleteAsync(cancellationToken);
            
            _logger.LogInformation("🚗 Vehicle {Plate} status updated to 'in_use' after delivery {Code} was created", 
                notification.VehiclePlate, notification.Code);
        }
    }
}

