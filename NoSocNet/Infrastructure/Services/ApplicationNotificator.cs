using NoSocNet.BLL.Abstractions;
using NoSocNet.BLL.Enums;
using NoSocNet.BLL.Models;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using NoSocNet.Extensions;
using NoSocNet.Infrastructure.Services.Hub;
using NoSocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{
    public class ApplicationNotificator : HubBase, INotificator<string>
    {
        public ApplicationNotificator()
        {

        }
        public Task<bool> Notificate(INotification<string> notification)
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
            NewChatUser<User, string> userJoin = input as NewChatUser<User, string>;
            ChatJoinViewModel vm = null;

            if (userJoin != null)
            {
                vm = new ChatJoinViewModel
                {
                    ChatId = userJoin.Room.Id,
                    ChatName = userJoin.Room.RoomName,
                    User = new UserViewModel
                    {
                        Id = userJoin.User.Id,
                        Email = userJoin.User.Email,
                        UserName = userJoin.User.UserName
                    }
                };
            }

            return new HubNotification(HubNotificationType.ChatJoin, vm);
        }

        private HubNotification FormMessageNotification(Object input)
        {
            Message<User, string> message = input as Message<User, string>;
            MessageViewModel vm = null;

            if (message != null)
            {
                vm = new MessageViewModel
                {
                    ChatRoomId = message.ChatRoomId,
                    Id = message.Id,
                    SendDate = message.SendDate,
                    SenderId = message.SenderId,
                    Text = message.Text,
                    SenderUserName = message.Sender.UserName
                };
            }

            return new HubNotification(HubNotificationType.Message, vm);
        }

        private HubNotification FormNewChatNotification(Object input)
        {
            ChatRoom<User, string> room = input as ChatRoom<User, string>;
            ChatRoomViewModel vm = null;

            if (room != null)
            {
                vm = new ChatRoomViewModel
                {
                    Id = room.Id,
                    OwnerId = room.OwnerId,
                    IsPrivate = room.IsPrivate,
                    RoomName = room.RoomName,
                    Participants = room.Participants.Select(x => new UserViewModel
                    {
                        Email = x.Email,
                        UserName = x.UserName,
                        Id = x.Id
                    }).ToList()
                };
            }

            return new HubNotification(HubNotificationType.NewChat, vm);
        }
    }
}
