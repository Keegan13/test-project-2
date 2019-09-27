using System;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub.Abstractions
{
    public interface IHub
    {
        Connection Connect(Guid connectionId);

        Connection Connect(string userId);

        void Notify(INotification notification);
    }
}
