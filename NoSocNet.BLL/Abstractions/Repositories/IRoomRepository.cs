using NoSocNet.BLL.Models;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IChatRoomRepository<TUser, TKey>
    {
        Task<ChatRoom<TUser, TKey>> GetRoom(string roomId);
        Task<ChatRoom<TUser, TKey>> GetPrivateRoom(TKey firstUserId, TKey secondUserId);
        Task CreatePrivateRoom(TKey firstUserId, TKey secondUserId);
        Task<PagedList<ChatRoom<TUser, TKey>>> GetRoomsAsync(TKey currentUserId, FilterBase filter = null, Paginator pagination = null);
        Task AddUserToRoom(string userId, string roomId);
    }
}
