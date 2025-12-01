using movesys_backend_.Shared.Domain.Model.Events;

namespace movesys_backend_.Fleet.Domain.Model.Events;

/// <summary>
/// Evento que se dispara cuando se crea un nuevo vehículo
/// </summary>
public class VehicleCreatedEvent : IEvent
{
    public long VehicleId { get; }
    public string Plate { get; }
    public string Brand { get; }
    public string Model { get; }
    public string Status { get; }
    
    public VehicleCreatedEvent(long vehicleId, string plate, string brand, string model, string status)
    {
        VehicleId = vehicleId;
        Plate = plate;
        Brand = brand;
        Model = model;
        Status = status;
    }
}

