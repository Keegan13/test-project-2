using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class HubBase
    {
        protected static ConcurrentDictionary<string, List<Guid>> Connections = new ConcurrentDictionary<string, List<Guid>>();
        protected virtual event EventHandler<HubNotificationArguments> _event;
        protected static ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public HubBase()
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

        public virtual bool Subscribe(Guid connectionId, EventHandler<HubNotificationArguments> handler)
        {
            if (handler != null && Connections.Any(x => x.Value.Any(c => c == connectionId)))
            {
                this._event += handler;
                return true;
            }
            return false;
        }

        public virtual void Unsubscribe(EventHandler<HubNotificationArguments> handler)
        {
            this._event -= handler;
        }

        public void Notify(Object message, params string[] usersIds)
        {
            List<Guid> destinationsIds = new List<Guid>();

            foreach (var userId in usersIds)
            {
                if (Connections.TryGetValue(userId, out List<Guid> connections))
                {
                    destinationsIds.AddRange(connections);
                }
            }

            this._event.Invoke(this, new HubNotificationArguments(message, destinationsIds) { Type = NotificationType.Message });
            _resetEvent.Set();
        }
    }
}
