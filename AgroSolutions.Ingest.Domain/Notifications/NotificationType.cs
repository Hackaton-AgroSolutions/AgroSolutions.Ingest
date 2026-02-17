using System.ComponentModel;

namespace AgroSolutions.Ingest.Domain.Notifications;

public enum NotificationType : byte
{
    [Description("The client id and/or client secret do not match")] InvalidSensorCredentials
}
