

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{
    public class ApplicationUserStore : IApplicationUserStore<User>
    {
        private readonly UserManager<User> userManager;

        public ApplicationUserStore(
            UserManager<User> userManager
            )
        {
            this.userManager = userManager;
        }
        public async Task<IList<User>> GetUsersAsync()
        {
            return await this.userManager.Users.ToListAsync();
        }
    }
}
