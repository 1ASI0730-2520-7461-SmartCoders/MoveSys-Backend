using Cortex.Mediator.Notifications;

namespace movesys_backend_.Shared.Domain.Model.Events;

/// <summary>
/// Interfaz base para eventos de dominio
/// Extiende INotification para integrarse con Cortex.Mediator
/// </summary>
public interface IEvent : INotification
{
}

