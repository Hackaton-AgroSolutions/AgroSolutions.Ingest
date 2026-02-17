using AgroSolutions.Ingest.Domain.Common;

namespace AgroSolutions.Ingest.Domain.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken, string? correlationId = default);
}
