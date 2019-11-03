using NoSocNet.Core.Models;
using System.Threading.Tasks;

namespace NoSocNet.Core.Interfaces
{
    public interface IChatService
    {
        Task<MessageDto> AddMessage(string userId, string roomId, string Message);

        Task<ChatRoomDto> GetOrCreatePrivateRoomWith(string userId);

        Task<ChatRoomDto> InviteToRoomAsync(string userId, string roomId);

        Task SetReadByUserAsync(string userId, string roomId, int? tillMessageId = null);
    }
}
