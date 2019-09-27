using NoSocNet.Infrastructure.Services.Hub.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public abstract class HubBase
    {
        protected static ConcurrentDictionary<string, List<Guid>> UserConnections = new ConcurrentDictionary<string, List<Guid>>();
        protected static ConcurrentDictionary<Guid, Connection> Connections = new ConcurrentDictionary<Guid, Connection>();

        public HubBase()
        {

        }

        public virtual Connection Connect(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Value cannot be null or empty");
            }

            Connection connection = new Connection(userId);

            UserConnections.AddOrUpdate(userId, (uid) => new List<Guid>() { connection.ConnectionId }, (uid, connections) =>
            {
                connections.Add(connection.ConnectionId);
                return connections;
            });

            Connections.AddOrUpdate(connection.ConnectionId, connection, (oldGuid, oldConn) => connection);

            return connection;
        }

        public virtual Connection Connect(Guid connectionId)
        {
            if (Connections.TryGetValue(connectionId, out Connection value))
            {
                return value;
            }

            return null;
        }

        public void Notify(Object message, params string[] usersIds)
        {
            ICollection<Guid> destinationsIds = new HashSet<Guid>();

            foreach (var userId in usersIds)
            {
                var connIds = UserConnections.GetValueOrDefault(userId);
                if (connIds != null)
                {
                    foreach (var connId in connIds)
                    {
                        destinationsIds.Add(connId);
                    }
                }
            }

            foreach (var connId in destinationsIds)
            {
                var conn = Connections.GetValueOrDefault(connId);
                if (conn != null)
                {
                    conn.Push(new NotificationBase(connId, message));
                }
            }
        }
    }
}
