using NoSocNet.BLL.Abstractions;
using NoSocNet.BLL.Models;
using NoSocNet.DAL.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface INotificator<TUserKey>
    {
        Task<bool> Notificate(INotification<TUserKey> notification);
    }
}