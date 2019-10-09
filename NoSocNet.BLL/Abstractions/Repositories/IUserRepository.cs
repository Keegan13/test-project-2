using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IUserRepository<TUser, TKey>
    {
        Task<PagedList<TUser>> GetPrivateRoomSuplementAsync(TKey userId, FilterBase filter = null, Paginator paginator = null);

        Task<PagedList<TUser>> GetRoomUserSuplementAsync(TKey key, string roomId, FilterBase filter = null, Paginator paginator = null);

        Task<TUser> GetByIdAsync(TKey userId);

        Task<bool> Exists(TKey userId);
    }
}

