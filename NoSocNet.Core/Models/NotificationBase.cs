using NoSocNet.Core.Enums;
using NoSocNet.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace NoSocNet.Core.Models
{
    public abstract class NotificationBase : INotification
    {
        public NotificationBase(object body, NotificationType type, IEnumerable<string> receivers)
        {
            Type = type;
            Receivers = receivers.ToArray();
            Body = body;
        }

        public NotificationType Type { get; protected set; }

        public IEnumerable<string> Receivers { get; protected set; }

        public virtual object Body { get; protected set; }
    }
}
