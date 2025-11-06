using Cortex.Mediator;
using movesys_backend_.Deliveries.Domain.Model.Aggregates;
using movesys_backend_.Deliveries.Domain.Model.Events;
using movesys_backend_.Deliveries.Domain.Repositories;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.Deliveries.Application.Internal.CommandServices;

public interface IDeliveryCommandService
{
    Task<Delivery> CreateAsync(Delivery delivery, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, Delivery delivery, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}

public class DeliveryCommandService : IDeliveryCommandService
{
    private readonly IDeliveryRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;
    
    public DeliveryCommandService(
        IDeliveryRepository repo, 
        IUnitOfWork uow,
        IMediator mediator)
    {
        _repo = repo; 
        _uow = uow;
        _mediator = mediator;
    }

    public async Task<Delivery> CreateAsync(Delivery delivery, CancellationToken ct = default)
    {
        await _repo.AddAsync(delivery, ct);
        await _uow.CompleteAsync(ct);
        
        // Disparar evento de dominio
        var deliveryCreatedEvent = new DeliveryCreatedEvent(
            delivery.Id,
            delivery.Code,
            delivery.VehiclePlate,
            delivery.DriverName,
            delivery.Status,
            delivery.DistanceKm
        );
        await _mediator.PublishAsync(deliveryCreatedEvent, ct);
        
        return delivery;
    }

    public async Task<bool> UpdateAsync(long id, Delivery delivery, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        existing.Code = delivery.Code;
        existing.CustomerName = delivery.CustomerName;
        existing.Address = delivery.Address;
        existing.OriginProvince = delivery.OriginProvince;
        existing.DestinationProvince = delivery.DestinationProvince;
        existing.ScheduledAt = delivery.ScheduledAt;
        existing.Status = delivery.Status;
        existing.VehicleId = delivery.VehicleId;
        existing.VehiclePlate = delivery.VehiclePlate;
        existing.DriverName = delivery.DriverName;
        existing.EtaMinutes = delivery.EtaMinutes;
        existing.DistanceKm = delivery.DistanceKm;
        _repo.Update(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var existing = await _repo.FindByIdAsync(id, ct);
        if (existing is null) return false;
        _repo.Remove(existing);
        await _uow.CompleteAsync(ct);
        return true;
    }
}


