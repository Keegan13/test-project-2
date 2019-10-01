using System;
using System.Collections.Generic;
using System.Linq;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class HubNotificationArguments
    {
        public HubNotificationArguments(object message, IEnumerable<Guid> destinations)
        {
            this.Message = message;
            this.Connections = destinations.Distinct();
        }

        public NotificationType Type { get; set; }
        public object Message { get; private set; }

        public IEnumerable<Guid> Connections { get; private set; }
    }
}
