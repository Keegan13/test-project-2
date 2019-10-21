using NoSocNet.BLL.Models;
using System;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IUserRepository<TUser, TKey>
    {
        [Obsolete]
        Task<PagedList<TUser>> GetPrivateRoomSuplementAsync(string userId, FilterBase filter = null, Paginator pagination = null);

        Task<PagedList<TUser>> ListAsync(FilterBase filter, Paginator paginator);

        Task<TUser> GetByIdAsync(TKey userId);

        Task<bool> ExistsAsync(TKey userId);
    }
}

