using System;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Core.Interfaces
{
    public interface INotificator
    {
        Task<bool> Notificate(INotification notification);
    }
}