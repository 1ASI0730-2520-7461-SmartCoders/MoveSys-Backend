using Cortex.Mediator;
using movesys_backend_.Fleet.Domain.Model.Aggregates;
using movesys_backend_.Fleet.Domain.Model.Events;
using movesys_backend_.Fleet.Domain.Repositories;
using movesys_backend_.Shared.Domain.Repositories;

namespace movesys_backend_.Fleet.Application.Internal.CommandServices;

public class VehicleCommandService : IVehicleCommandService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public VehicleCommandService(
        IVehicleRepository vehicleRepository, 
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Vehicle> CreateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);
        
        // Disparar evento de dominio
        var vehicleCreatedEvent = new VehicleCreatedEvent(
            vehicle.Id,
            vehicle.Plate,
            vehicle.Brand,
            vehicle.Model,
            vehicle.Status
        );
        await _mediator.PublishAsync(vehicleCreatedEvent, cancellationToken);
        
        return vehicle;
    }

    public async Task<bool> UpdateAsync(long id, Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        var existing = await _vehicleRepository.FindByIdAsync(id, cancellationToken);
        if (existing is null) return false;

        existing.Plate = vehicle.Plate;
        existing.Brand = vehicle.Brand;
        existing.Model = vehicle.Model;
        existing.Year = vehicle.Year;
        existing.Color = vehicle.Color;
        existing.Type = vehicle.Type;
        existing.Capacity = vehicle.Capacity;
        existing.FuelType = vehicle.FuelType;
        existing.Status = vehicle.Status;
        existing.CurrentDriver = vehicle.CurrentDriver;
        existing.Mileage = vehicle.Mileage;

        _vehicleRepository.Update(existing);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var existing = await _vehicleRepository.FindByIdAsync(id, cancellationToken);
        if (existing is null) return false;
        _vehicleRepository.Remove(existing);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return true;
    }
}


