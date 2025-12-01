using Cortex.Mediator.Notifications;
using Microsoft.Extensions.Logging;
using movesys_backend_.Shared.Domain.Model.Events;

namespace movesys_backend_.Shared.Application.Internal.EventHandlers;

/// <summary>
/// Handler de logging automático para todos los eventos
/// </summary>
public class LoggingEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : class, INotification
{
    private readonly ILogger<LoggingEventHandler<TEvent>> _logger;
    
    public LoggingEventHandler(ILogger<LoggingEventHandler<TEvent>> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("✅ Event raised: {EventType} - {EventData}", 
            typeof(TEvent).Name, 
            System.Text.Json.JsonSerializer.Serialize(notification));
        return Task.CompletedTask;
    }
}

