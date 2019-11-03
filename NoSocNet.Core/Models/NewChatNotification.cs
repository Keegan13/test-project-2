using NoSocNet.Core.Enums;
using System.Collections.Generic;

namespace NoSocNet.Core.Models
{
    public class NewChatNotification : NotificationBase
    {
        public NewChatNotification(ChatRoomDto newRoom, IEnumerable<string> receivers) : base(newRoom, NotificationType.NewChat, receivers)
        {

        }
    }
}
