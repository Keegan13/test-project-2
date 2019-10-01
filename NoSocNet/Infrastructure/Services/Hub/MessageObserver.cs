using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class MessageObserver
    {
        private readonly HubMessageSender hub;
        private readonly ManualResetEvent notificator;

        public Guid ConnectionId { get; private set; }
        private Object message = null;
        private bool receivedMessage = false;

        public MessageObserver(
            HubMessageSender hub
            )
        {
            this.hub = hub;
            this.notificator = hub.GetNotificator();
        }

        public Object GetMessageOrDefaultAsync(Guid connectionId, int? timeOut = null)
        {
            this.ConnectionId = connectionId;
            DateTime globTimeout = timeOut.HasValue ? DateTime.Now.AddMilliseconds(timeOut.Value) : DateTime.Now.AddDays(1);
            var handler = new EventHandler<HubNotificationArguments>(onMessageHandler);

            this.hub.Subscribe(this.ConnectionId, handler);

            try
            {
                while (!this.receivedMessage && DateTime.Now < globTimeout)
                {
                    if (timeOut.HasValue)
                    {
                        notificator.WaitOne(timeOut.Value);
                    }
                    else
                    {
                        notificator.WaitOne();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.hub.Unsubscribe(handler);
            }

            return this.message;
        }

        private void onMessageHandler(object sender, HubNotificationArguments e)
        {
            if (e.Connections.Contains(this.ConnectionId))
            {
                this.message = e.Message;
                this.receivedMessage = true;
            }
        }
    }
}

