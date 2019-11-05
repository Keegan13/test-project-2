using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class MessageObserver
    {
        public Guid ConnectionId { get; private set; }

        private readonly ApplicationNotificator hub;
        private readonly ManualResetEvent notificator;
        private HubNotification Notification = null;

        private bool notificationReceived = false;

        public MessageObserver(
            ApplicationNotificator hub
            )
        {
            this.hub = hub;
            this.notificator = hub.GetNotificator();
        }

        public virtual HubNotification GetMessageOrDefaultAsync(Guid connectionId, int? timeOut = null)
        {
            this.ConnectionId = connectionId;

            DateTime globTimeout = timeOut.HasValue ? DateTime.Now.AddMilliseconds(timeOut.Value) : DateTime.Now.AddDays(1);
            var handler = new EventHandler<HubNotificationArguments>(onMessageHandler);

            this.hub.Subscribe(this.ConnectionId, handler);

            try
            {
                while (!this.notificationReceived && DateTime.Now < globTimeout)
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

            return this.Notification;
        }

        private void onMessageHandler(object sender, HubNotificationArguments e)
        {
            if (e.Connections.Contains(this.ConnectionId))
            {
                this.Notification = e.Notification;
                this.notificationReceived = true;
            }
        }
    }
}

