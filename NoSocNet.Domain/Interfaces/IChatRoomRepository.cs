using NoSocNet.Domain.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace NoSocNet.Domain.Interfaces
{
    public interface IChatRoomRepository : IRepository<ChatRoomEntity, string>
    {
        Task<ChatRoomEntity> GetPrivateRoomAsync(string firstUserId, string secondUserId);
        Task CreatePrivateRoomAsync(string firstUserId, string secondUserId);
        Task AddParticipantsAsync(string chatRoomId, IEnumerable<string> usersIds);

        Task<IEnumerable<ChatRoomEntity>> GetRecentChatRoomsAsync(string userId, string[] skipIds, int count = 10);

        Task<IDictionary<string, bool>> GetHasUnreadAsync(string[] chatRoomsIds, string userId);

        Task<IEnumerable<ChatRoomEntity>> SearchRoomsAsync(string userId, string keywords, int take = 10, int skip = 0);
    }
}
