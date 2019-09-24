using NoSocNet.BLL.Models;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IHubSender<TUser, TKey>
    {
        Task<bool> PushMessage(Message<TUser, TKey> message, string chatRoomId);
    }
}
