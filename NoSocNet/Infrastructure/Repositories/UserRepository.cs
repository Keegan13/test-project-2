using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository<User, string>
    {
        private readonly DbContext context;
        private DbSet<User> Users => context.Set<User>();

        public UserRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<PagedList<User>> GetPrivateRoomSuplementAsync(string userId, FilterBase filter = null, Paginator pagination = null)
        {
            var query = Users
                .Where(x => x.Id != userId && x.UserRooms.Where(ur => ur.ChatRoom.IsPrivate).All(ur => !ur.ChatRoom.UserRooms.Any(r => r.UserId == userId)));

            query = ApplyFilter(query, filter);
            int count = await query.CountAsync();
            query = ApplyPagination(query, pagination);
            var data = await query.ToListAsync();

            return new PagedList<User>(data, count, filter, pagination);
        }

        private IQueryable<User> ApplyPagination(IQueryable<User> query, Paginator paginator)
        {
            if (paginator == null)
            {
                paginator = new Paginator
                {
                    Page = 1,
                    PageSize = 10,
                    SortColumn = "",
                    SortOrder = SortOrder.Descending,
                };
            }

            query = query
                    .Skip((paginator.Page - 1) * paginator.PageSize)
                    .Take(paginator.PageSize);

            return query;
        }

        private IQueryable<User> ApplyFilter(IQueryable<User> query, FilterBase filter)
        {
            if (!String.IsNullOrEmpty(filter?.Keywords))
            {
                query = query.Where(x => x.UserName.Contains(filter.Keywords));
            }

            return query;
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            var user = await this.Users.FindAsync(userId);

            return user;
        }

        public async Task<bool> Exists(string userId)
        {
            var doesExists = await this.Users.AnyAsync(x => x.Id == userId);

            return doesExists;
        }

        public async Task<PagedList<User>> GetRoomUserSuplementAsync(string userId, string roomId, FilterBase filter = null, Paginator paginator = null)
        {
            var query = Users
                .Where(x => x.Id != userId)
                .Where(x => x.UserRooms.All(ur => ur.ChatRoomId != roomId));

            query = ApplyFilter(query, filter);
            int count = await query.CountAsync();
            query = ApplyPagination(query, paginator);
            var data = await query.ToListAsync();

            return new PagedList<User>(data, count, filter, paginator);
        }
    }
}
