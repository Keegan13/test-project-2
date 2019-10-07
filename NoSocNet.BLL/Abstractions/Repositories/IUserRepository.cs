using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IUserRepository<TUser, TKey>
    {
        Task<PagedList<TUser>> GetPrivateRoomSuplementAsync(TKey userId, FilterBase filter = null, Paginator paginator = null);
    }
}

