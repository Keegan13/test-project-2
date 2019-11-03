using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoSocNet.Domain.Models;
using NoSocNet.Infrastructure.Domain;
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
        private UserManager<UserEntity> userManager;

        public UserFactory(UserManager<UserEntity> userManager, ApplicationDbContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<UserEntity>> AddUsers(int count)
        {
            IEnumerable<UserEntity> users = (await GenerateUsers(count)).ToList();
            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "123456Qq@");
            }

            return users;
        }

        public async Task<IEnumerable<UserEntity>> GenerateUsers(int count)
        {
            var index = await LastIndexAsync();

            List<UserEntity> newUsers = new List<UserEntity>(count);
            for (int i = 1; i < count + 1; i++)
            {
                newUsers.Add(new UserEntity
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

            var regex = new Regex("test(?<MockNumber>[0-9]+)@mail.com");
            var index = usedEmails.Max(x =>
            {
                var match = regex.Match(x).Groups["MockNumber"];

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
