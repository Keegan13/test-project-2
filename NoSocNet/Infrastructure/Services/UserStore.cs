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

        public Task<bool> Exists(string userId)
        {
            var doesExists = this.userManager.Users.AnyAsync(x => x.Id == userId);

            return doesExists;
        }

        public IQueryable<User> Query()
        {
            return this.userManager.Users;
        }

        public async Task<IList<User>> GetUserListAsync()
        {
            var userList = await this.userManager.Users.ToListAsync();

            return userList;
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            return user;
        }
    }
}
