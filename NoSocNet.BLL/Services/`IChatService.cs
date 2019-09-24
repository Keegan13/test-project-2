using NoSocNet.BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IChatService<TUser, TKey>
    {
        Task<Message<TUser, TKey>> Push(string userId, string roomId, string Message);

        Task<IList<Message<TUser, TKey>>> GetChatMessages(string chatRoom);

        Task<IList<ChatRoom<TUser, TKey>>> GetUserRooms(TKey userId);

        Task<ChatRoom<TUser, TKey>> JoinPrivateAsync(TKey userId);

        Task<ChatRoom<TUser, TKey>> InviteToRoom(TKey userId, string roomId);

        Task<ChatRoom<TUser, TKey>> GetRoomAsync(string roomId);
    }
}
