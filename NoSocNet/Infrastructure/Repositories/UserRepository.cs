using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.BLL.Enums;
using NoSocNet.BLL.Models;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
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

        [Obsolete]

        //ToDo: change or let it stay
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

        public async Task<PagedList<User>> ListAsync(FilterBase filter, Paginator paginator)
        {
            IQueryable<User> query = Users.AsQueryable();
            query = ApplyFilter(query, filter);
            int count = await query.CountAsync();

            if (!String.IsNullOrWhiteSpace(paginator.TailId))
            {
                string[] ids = await query.Select(x => x.Id).ToArrayAsync();
                int skip = Array.IndexOf(ids, paginator.TailId);
                skip = skip > 0 ? skip + 1 : 0;
                query = query.Skip(skip)
                    .Take(paginator?.PageSize > 0 ? paginator.PageSize : 10);
            }
            else
            {
                //simple pagination here
                query = ApplyPagination(query, paginator);
            }

            List<User> data = await query.ToListAsync();

            return new PagedList<User>(data, count, filter, paginator);
        }

        private IQueryable<User> ApplyPagination(IQueryable<User> query, Paginator paginator)
        {
            query = query
                    .Skip((paginator.Page - 1) * paginator.PageSize)
                    .Take(paginator.PageSize);

            return query;
        }

        private IQueryable<User> ApplyFilter(IQueryable<User> query, FilterBase filter)
        {
            if (!String.IsNullOrWhiteSpace(filter.ChatRoomId))
            {

                switch (filter.Type)
                {
                    case ParticipantsType.Participant:
                        query = query.Where(x => x.UserRooms.Any(ur => ur.ChatRoomId == filter.ChatRoomId));
                        break;
                    case ParticipantsType.NonParticipant:
                        query = query.Where(x => x.UserRooms.All(ur => ur.ChatRoomId != filter.ChatRoomId));
                        break;
                    default:
                        if (!String.IsNullOrWhiteSpace(filter.CurrentUserId))
                        {
                            query = query.Where(x => x.Id != filter.CurrentUserId && x.UserRooms.Where(ur => ur.ChatRoom.IsPrivate).All(ur => !ur.ChatRoom.UserRooms.Any(r => r.UserId == filter.CurrentUserId)));
                        }
                        break;
                }

            }

            if (!String.IsNullOrWhiteSpace(filter.Keywords))
            {
                query = query.Where(x => x.Email.Contains(filter.Keywords) || x.UserName.Contains(filter.Keywords));
            }

            return query;
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            var user = await this.Users.FindAsync(userId);

            return user;
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            var doesExists = await this.Users.AnyAsync(x => x.Id == userId);

            return doesExists;
        }
    }
}
