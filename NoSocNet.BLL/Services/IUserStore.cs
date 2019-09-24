using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IApplicationUserStore<TUser>
    {
        Task<IList<TUser>> GetUsersAsync();
    }
}
