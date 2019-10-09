using NoSocNet.BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IMessageRepository<TUser, TKey>
    {
        Task SetReadByUserAsync(TKey userId, string roomId, int? tillMessageId = null);

        Task<PagedList<Message<TUser, TKey>>> GetMessagesAsync(string chatRoomId, FilterBase filter = null, Paginator paginator = null);

        Task<IEnumerable<Message<TUser, TKey>>> GetMessagesAsync(string roomId, int size, int? tailId = null);

        Task AddMessage(Message<TUser, TKey> message);
    }
}
