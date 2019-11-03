using System;
using System.Collections.Generic;
using System.Linq;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class HubNotificationArguments
    {
        public HubNotificationArguments(HubNotification notification, IEnumerable<Guid> destinations)
        {
            this.Notification = notification;
            this.Connections = destinations.Distinct();
        }

        public HubNotification Notification { get; private set; }

        public IEnumerable<Guid> Connections { get; private set; }
    }
}
