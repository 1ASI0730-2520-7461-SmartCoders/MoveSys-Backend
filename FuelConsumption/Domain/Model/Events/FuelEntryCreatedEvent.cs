using movesys_backend_.Shared.Domain.Model.Events;

namespace movesys_backend_.FuelConsumption.Domain.Model.Events;

/// <summary>
/// Domain event raised when a fuel entry is created
/// </summary>
public class FuelEntryCreatedEvent : IEvent
{
    public long FuelEntryId { get; }
    public string VehiclePlate { get; }
    public decimal Liters { get; }
    public decimal Cost { get; }
    
    public FuelEntryCreatedEvent(long fuelEntryId, string vehiclePlate, decimal liters, decimal cost)
    {
        FuelEntryId = fuelEntryId;
        VehiclePlate = vehiclePlate;
        Liters = liters;
        Cost = cost;
    }
}

