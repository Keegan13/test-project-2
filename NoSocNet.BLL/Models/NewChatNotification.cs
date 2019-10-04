using NoSocNet.BLL.Enums;
using NoSocNet.BLL.Models;
using System.Collections.Generic;

namespace NoSocNet.BLL.Models
{
    public class NewChatNotification<TUser, TKey> : NotificationBase<TKey>
    {
        public NewChatNotification(ChatRoom<TUser, TKey> newRoom, IEnumerable<TKey> receivers) : base(newRoom, NotificationType.NewChat, receivers)
        {

        }
    }
}
