using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Notificator
{
    public class TypedNotification
    {
        public TypedNotification(AppNotificationType type, Object notificationBody)
        {
            this.Type = type;
            this.Notification = notificationBody;
        }
        public AppNotificationType Type { get; private set; }

        public object Notification { get; private set; }
    }
}
