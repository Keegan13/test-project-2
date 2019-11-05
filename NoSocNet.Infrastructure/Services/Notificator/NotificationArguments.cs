using System;
using System.Collections.Generic;
using System.Linq;

namespace NoSocNet.Infrastructure.Services.Notificator
{
    public class NotificationArguments
    {
        public NotificationArguments(TypedNotification notification, IEnumerable<Guid> destinations)
        {
            this.Notification = notification;
            this.Connections = destinations.Distinct();
        }

        public TypedNotification Notification { get; private set; }

        public IEnumerable<Guid> Connections { get; private set; }
    }
}
