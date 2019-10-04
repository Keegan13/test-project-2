using NoSocNet.BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IChatService<TUser, TKey>
    {
        Task<Message<TUser, TKey>> Push(string userId, string roomId, string Message);

        Task<IList<ChatRoom<TUser, TKey>>> GetRoomsByUserAsync(TKey userId);

        Task<ChatRoom<TUser, TKey>> GetOrCreatePrivateRoomWith(TKey userId);

        Task<ChatRoom<TUser, TKey>> InviteToRoomAsync(TKey userId, string roomId);

        Task<ChatRoom<TUser, TKey>> GetRoomAsync(string roomId);

        Task SetReadByUserAsync(TKey userId, string roomId, int? tillMessageId = null);

        Task<List<TUser>> GetPrivateRoomSupplementFor(TKey userId);
    }
}
