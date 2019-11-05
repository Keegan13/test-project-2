using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Notificator
{
    public class NotificationObserver
    {
        public Guid ConnectionId { get; private set; }

        private readonly NotificationService notificationService;
        private readonly ManualResetEvent notificator;
        private TypedNotification Notification = null;

        private bool notificationReceived = false;

        public NotificationObserver(
            NotificationService notificationService
            )
        {
            this.notificationService = notificationService;
            this.notificator = notificationService.GetNotificator();
        }

        public virtual TypedNotification GetMessageOrDefaultAsync(Guid connectionId, int? timeOut = null)
        {
            this.ConnectionId = connectionId;

            DateTime globTimeout = timeOut.HasValue ? DateTime.Now.AddMilliseconds(timeOut.Value) : DateTime.Now.AddDays(1);
            var handler = new EventHandler<NotificationArguments>(onMessageHandler);

            this.notificationService.Subscribe(this.ConnectionId, handler);

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
                this.notificationService.Unsubscribe(handler);
            }

            return this.Notification;
        }

        private void onMessageHandler(object sender, NotificationArguments e)
        {
            if (e.Connections.Contains(this.ConnectionId))
            {
                this.Notification = e.Notification;
                this.notificationReceived = true;
            }
        }
    }
}

