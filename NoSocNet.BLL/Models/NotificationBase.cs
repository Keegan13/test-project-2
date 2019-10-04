using NoSocNet.BLL.Abstractions;
using NoSocNet.BLL.Enums;
using NoSocNet.BLL.Services;
using System.Collections.Generic;
using System.Linq;

namespace NoSocNet.BLL.Models
{
    public abstract class NotificationBase<TUserKey> : INotification<TUserKey>
    {
        public NotificationBase(object body, NotificationType type, IEnumerable<TUserKey> receivers)
        {
            Type = type;
            Receivers = receivers.ToArray();
            Body = body;
        }

        public NotificationType Type { get; protected set; }

        public IEnumerable<TUserKey> Receivers { get; protected set; }

        public virtual object Body { get; protected set; }
    }
}
