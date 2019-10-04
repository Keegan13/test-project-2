using NoSocNet.BLL.Enums;
using System.Collections.Generic;

namespace NoSocNet.BLL.Abstractions
{
    public interface INotification<TUserKey>
    {
        NotificationType Type { get; }
        IEnumerable<TUserKey> Receivers { get; }
        object Body { get; }
    }
}