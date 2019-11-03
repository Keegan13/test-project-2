using NoSocNet.Core.Enums;
using System.Collections.Generic;

namespace NoSocNet.Core.Interfaces
{
    public interface INotification
    {
        NotificationType Type { get; }
        IEnumerable<string> Receivers { get; }
        object Body { get; }
    }
}