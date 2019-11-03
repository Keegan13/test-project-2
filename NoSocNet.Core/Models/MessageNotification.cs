using NoSocNet.Core.Enums;
using System.Collections.Generic;

namespace NoSocNet.Core.Models
{
    public class MessageNotification: NotificationBase
    {
        public MessageNotification(MessageDto userJoin, IEnumerable<string> receivers) : base(userJoin, NotificationType.Message, receivers)
        {

        }
    }
}
