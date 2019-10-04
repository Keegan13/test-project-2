using NoSocNet.BLL.Enums;
using NoSocNet.BLL.Models;
using System.Collections.Generic;

namespace NoSocNet.BLL.Models
{
    public class MessageNotification<TUser, TKey> : NotificationBase<TKey>
    {
        public MessageNotification(Message<TUser, TKey> userJoin, IEnumerable<TKey> receivers) : base(userJoin, NotificationType.Message, receivers)
        {

        }
    }
}
