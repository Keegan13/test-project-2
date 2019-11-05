using NoSocNet.Core.Interfaces;
using NoSocNet.Infrastructure.Services.Notificator;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{

    public class NotificationService : ApplicationNotificator, INotificator
    {
        public Task<bool> Notificate(INotification notification)
        {
            try
            {
                base.Notify(new TypedNotification((AppNotificationType)notification.Type, notification.Body), notification.Receivers.ToArray());
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult(false);
            }
        }
    }
}
