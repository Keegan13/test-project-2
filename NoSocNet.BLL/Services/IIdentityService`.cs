using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IIdentityService<TUser>
    {
        string CurrentUserId { get; }
        Task<TUser> CurrentUser { get; }
    }
}
