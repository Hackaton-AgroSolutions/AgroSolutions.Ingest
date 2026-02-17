namespace AgroSolutions.Ingest.Domain.Notifications;

public record Notification(NotificationType Type, IEnumerable<object>? @Params = default);
