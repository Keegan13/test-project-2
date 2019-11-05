using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Notificator
{
    public class ApplicationNotificator
    {
        protected static ConcurrentDictionary<string, List<Guid>> Connections = new ConcurrentDictionary<string, List<Guid>>();
        protected virtual event EventHandler<NotificationArguments> _event;
        protected static ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public ApplicationNotificator()
        {

        }

        public ManualResetEvent GetNotificator()
        {
            return _resetEvent;
        }

        public virtual Guid Connect(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Value cannot be null or empty");
            }

            Guid connectionId = Guid.NewGuid();

            Connections.AddOrUpdate(userId, (uid) => new List<Guid>() { connectionId }, (uid, connections) =>
            {
                connections.Add(connectionId);
                return connections;
            });
            return connectionId;
        }

        public virtual bool Subscribe(Guid connectionId, EventHandler<NotificationArguments> handler)
        {
            if (handler != null && Connections.Any(x => x.Value.Any(c => c == connectionId)))
            {
                this._event += handler;
                return true;
            }
            return false;
        }

        public virtual void Unsubscribe(EventHandler<NotificationArguments> handler)
        {
            this._event -= handler;
        }

        public void Notify(TypedNotification notification, params string[] usersIds)
        {
            List<Guid> destinationsIds = new List<Guid>();

            foreach (var userId in usersIds)
            {
                if (Connections.TryGetValue(userId, out List<Guid> connections))
                {
                    destinationsIds.AddRange(connections);
                }
            }

            if (destinationsIds.Count() > 0)
            {
                this._event.Invoke(this, new NotificationArguments(notification, destinationsIds));
                _resetEvent.Set();
            }
        }
    }
}
