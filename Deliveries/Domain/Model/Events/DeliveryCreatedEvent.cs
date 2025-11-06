using movesys_backend_.Shared.Domain.Model.Events;

namespace movesys_backend_.Deliveries.Domain.Model.Events;

/// <summary>
/// Evento que se dispara cuando se crea una nueva entrega
/// </summary>
public class DeliveryCreatedEvent : IEvent
{
    public long DeliveryId { get; }
    public string Code { get; }
    public string? VehiclePlate { get; }
    public string? DriverName { get; }
    public string Status { get; }
    public double? DistanceKm { get; }
    
    public DeliveryCreatedEvent(
        long deliveryId, 
        string code, 
        string? vehiclePlate, 
        string? driverName, 
        string status,
        double? distanceKm)
    {
        DeliveryId = deliveryId;
        Code = code;
        VehiclePlate = vehiclePlate;
        DriverName = driverName;
        Status = status;
        DistanceKm = distanceKm;
    }
}