using AgroSolutions.Ingest.Domain.Notifications;
using AgroSolutions.Ingest.Infrastructure.Extensions;

namespace AgroSolutions.Ingest.API.Extensions;

public static partial class NotificationContextExtensions
{
    extension(INotificationContext notificationContext)
    {
        public IEnumerable<string> AsListString
            => notificationContext.Notifications.Select(n => string.Format(n.Type.GetDescription(), args: n?.Params?.ToArray() ?? []));
    }
}
