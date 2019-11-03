using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class HubNotification
    {
        public HubNotification(HubNotificationType type, Object notificationBody)
        {
            this.Type = type;
            this.Notification = notificationBody;
        }
        public HubNotificationType Type { get; private set; }

        public object Notification { get; private set; }
    }
}
