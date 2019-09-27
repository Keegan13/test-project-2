using System;

namespace NoSocNet.Infrastructure.Services.Hub.Abstractions
{
    public interface INotification
    {
        Guid ConnectionId { get; }
        object Body { get; }
    }
}
