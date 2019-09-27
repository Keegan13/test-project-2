using NoSocNet.Infrastructure.Services.Hub.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class NotificationBase : INotification
    {
        public NotificationBase(Guid connectionId, object body)
        {
            this.ConnectionId = connectionId;
            this.Body = body;
        }

        public virtual Guid ConnectionId { get; protected set; }
        public virtual object Body { get; protected set; }
    }
}
