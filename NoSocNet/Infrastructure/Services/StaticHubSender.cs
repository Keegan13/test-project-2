using NoSocNet.BLL.Models;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{
    public class StaticHubSender : IHubSender<User, string>
    {
        private static ConcurrentDictionary<string, Object> Users { get; set; }

        private readonly object locker = new object();
        public Task<bool> PushMessage(Message<User, string> message, string chatRoomId)
        {
            lock (locker)
            {
                return Task.FromResult(true);
            }
            //Todo: implement
        }

        protected class ChatUser
        {
            public HashSet<string> Connections = new HashSet<string>();
        }

        protected class ChatRoomConnection
        {
            public ChatRoomConnection(string roomId)
            {
                this.ConnectionId = Guid.NewGuid();
                this.RoomId = RoomId;
            }
            public Guid ConnectionId { get; private set; }
            public string RoomId { get; private set; }

            public HashSet<string> Connections { get; set; }
        }
    }
}
