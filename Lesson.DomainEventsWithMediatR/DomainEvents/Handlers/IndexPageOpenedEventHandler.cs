using Lesson.DomainEventsWithMediatR.DomainEvents.Events;
using MediatR;

namespace Lesson.DomainEventsWithMediatR.DomainEvents.Handlers;

public class IndexPageOpenedEventHandler : INotificationHandler<IndexPageOpened>, IDisposable
{
    private readonly ILogger<IndexPageOpenedEventHandler> _logger;
    private readonly ScopedDependency _scopedDependency;

    public IndexPageOpenedEventHandler(ILogger<IndexPageOpenedEventHandler> logger, ScopedDependency scopedDependency)
    {
        _logger = logger;
        _scopedDependency = scopedDependency;
    }

    public async Task Handle(IndexPageOpened notification, CancellationToken cancellationToken)
    {
        // Если сервер остановится, то cancellationToken НЕ сработает. 
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        _logger.LogInformation("Do some stuffff here");
    }

    public void Dispose()
    {
        //!!! DISPOSE может быть вызван до завершения метода Handle
        _logger.LogWarning("Disposing");
    }
}