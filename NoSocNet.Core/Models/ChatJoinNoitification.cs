using NoSocNet.Core.Enums;
using System.Collections.Generic;

namespace NoSocNet.Core.Models
{
    public class ChatJoinNoitification : NotificationBase
    {
        public ChatJoinNoitification(NewChatUser userJoin, IEnumerable<string> receivers) : base(userJoin, NotificationType.ChatJoin, receivers)
        {

        }
    }
}
