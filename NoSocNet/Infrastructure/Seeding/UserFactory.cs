using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoSocNet.DAL.Context;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Seeding
{
    public class UserFactory
    {
        private ApplicationDbContext context;
        private UserManager<User> userManager;

        public UserFactory(UserManager<User> userManager, ApplicationDbContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<User>> AddUsers(int count)
        {
            IEnumerable<User> users = (await GenerateUsers(count)).ToList();
            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "123456Qq@");
            }

            return users;
        }

        public async Task<IEnumerable<User>> GenerateUsers(int count)
        {
            var index = await LastIndexAsync();

            List<User> newUsers = new List<User>(count);
            for (int i = 1; i < count + 1; i++)
            {
                newUsers.Add(new User
                {
                    Email = String.Format("test{0}@mail.com", index + i),
                    UserName = String.Format("test{0}@mail.com", index + i)
                });
            }

            return newUsers;
        }

        public async Task<int> LastIndexAsync()
        {
            var usedEmails = await context.Users.Where(x => x.Email.StartsWith("test")).Select(x => x.Email).ToListAsync();

            var regex = new System.Text.RegularExpressions.Regex("test([0-9]+)@mail.com");
            var index = usedEmails.Max(x =>
            {
                var match = regex.Match(x).Groups.Skip(1).FirstOrDefault();
                if (int.TryParse(match?.Value, out int value))
                {
                    return value;
                }
                return 0;
            });
            return index > 0 ? index : 1;
        }
    }
}
