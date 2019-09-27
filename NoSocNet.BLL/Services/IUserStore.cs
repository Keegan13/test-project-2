using NoSocNet.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IApplicationUserStore<TUser>
    {
        Task<User> GetByIdAsync(string userId);
        Task<IList<TUser>> GetUserListAsync();
        Task<bool> Exists(string userId);
        IQueryable<User> Query();
    }
}
