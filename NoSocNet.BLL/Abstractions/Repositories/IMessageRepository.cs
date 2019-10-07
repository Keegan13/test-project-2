using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IMessageRepository<TUserKey>
    {
        Task SetReadByUserAsync(TUserKey userId, string roomId, int? tillMessageId = null);
    }
}
