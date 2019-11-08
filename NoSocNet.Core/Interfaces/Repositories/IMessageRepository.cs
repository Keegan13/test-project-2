using System.Collections.Generic;
using System.Threading.Tasks;
using NoSocNet.Domain.Models;

namespace NoSocNet.Core.Interfaces.Repositories
{
    public interface IMessageRepository : IRepository<MessageEntity, int>
    {
        Task SetReadByUserAsync(string userId, string chatRoomId, int? tillMessageId = null);

        Task<IEnumerable<MessageEntity>> GetMessagesAsync(string chatRoomId, int count, int? tailMessageId = null);
    }
}
