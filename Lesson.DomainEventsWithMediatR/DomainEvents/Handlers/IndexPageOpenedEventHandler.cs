using Lesson.DomainEventsWithMediatR.DomainEvents.Events;
using MediatR;

namespace Lesson.DomainEventsWithMediatR.DomainEvents.Handlers;

public class IndexPageOpenedEventHandler : INotificationHandler<IndexPageOpened>
{
    private readonly ILogger<IndexPageOpenedEventHandler> _logger;

    public IndexPageOpenedEventHandler(ILogger<IndexPageOpenedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(IndexPageOpened notification, CancellationToken cancellationToken)
    {
        // Если сервер остановится, то cancellationToken НЕ сработает. 
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        _logger.LogInformation("Do some stuffff here");
    }
}