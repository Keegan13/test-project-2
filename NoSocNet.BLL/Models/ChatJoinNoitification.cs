using NoSocNet.BLL.Enums;
using NoSocNet.BLL.Models;
using System.Collections.Generic;

namespace NoSocNet.BLL.Models
{
    public class ChatJoinNoitification<TUser, TKey> : NotificationBase<TKey>
    {
        public ChatJoinNoitification(NewChatUser<TUser, TKey> userJoin, IEnumerable<TKey> receivers) : base(userJoin, NotificationType.ChatJoin, receivers)
        {

        }
    }
}
