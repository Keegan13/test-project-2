using NoSocNet.BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IChatService<TUser, TKey>
    {
        Task<Message<TUser, TKey>> AddMessage(string userId, string roomId, string Message);

        Task<ChatRoom<TUser, TKey>> GetOrCreatePrivateRoomWith(TKey userId);

        Task<ChatRoom<TUser, TKey>> InviteToRoomAsync(TKey userId, string roomId);

        Task SetReadByUserAsync(TKey userId, string roomId, int? tillMessageId = null);
    }
}
