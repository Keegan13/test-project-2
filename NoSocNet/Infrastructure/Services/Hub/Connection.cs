using NoSocNet.BLL.Services;
using NoSocNet.Infrastructure.Services.Hub.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class Connection
    {
        private readonly Guid connectionId;

        public event EventHandler<MessageEventArguments> onMessage = new EventHandler<MessageEventArguments>((sender, args) => { });

        public Connection(string userId)
        {
            this.connectionId = Guid.NewGuid();
            this.UserId = UserId;
        }

        public string UserId { get; private set; }
        public Guid ConnectionId => this.connectionId;

        public void Push(INotification notification)
        {
            if (notification.ConnectionId == this.connectionId)
            {
                this.onMessage.Invoke(this, new MessageEventArguments(notification.Body));
            }
        }
    }
}
