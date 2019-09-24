using NoSocNet.BLL.Models;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IRoomStore<TUser, TKey>
    {
        Task<ChatRoom<TUser, TKey>> GetRoomAsync(string roomId);
    }
}
