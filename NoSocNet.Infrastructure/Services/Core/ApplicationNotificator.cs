using NoSocNet.Core.Enums;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Models;
using NoSocNet.Infrastructure.Services.Hub;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{

    public class ApplicationNotificator : HubBase, INotificator
    {
        public ApplicationNotificator()
        {

        }
        public Task<bool> Notificate(INotification notification)
        {
            try
            {
                switch (notification.Type)
                {
                    case NotificationType.Message:
                        base.Notify(FormMessageNotification(notification.Body), notification.Receivers.ToArray());
                        break;
                    case NotificationType.ChatJoin:
                        base.Notify(FormChatJoinNotification(notification.Body), notification.Receivers.ToArray());
                        break;
                    case NotificationType.NewChat:
                        base.Notify(FormNewChatNotification(notification.Body), notification.Receivers.ToArray());
                        break;
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

        private HubNotification FormChatJoinNotification(Object input)
        {
            NewChatUser userJoin = input as NewChatUser;

            return new HubNotification(HubNotificationType.ChatJoin, userJoin);
        }

        private HubNotification FormMessageNotification(Object input)
        {
            MessageDto message = input as MessageDto;

            return new HubNotification(HubNotificationType.Message, message);
        }

        private HubNotification FormNewChatNotification(Object input)
        {
            ChatRoomDto room = input as ChatRoomDto;

            return new HubNotification(HubNotificationType.NewChat, room);
        }
    }
}
