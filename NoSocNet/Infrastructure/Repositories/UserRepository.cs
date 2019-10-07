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

        public UserRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<PagedList<User>> GetPrivateRoomSuplementAsync(string userId, FilterBase filter = null, Paginator pagination = null)
        {
            var query = context
                .Set<User>()
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
                    SortOrder = SortOrder.Descending
                };
            }

            //ToDo: OrderByColumnName extesniosn method

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
    }
}
