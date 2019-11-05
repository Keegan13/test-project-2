using Microsoft.EntityFrameworkCore;
using NoSocNet.Core.Interfaces;
using NoSocNet.Domain.Models;
using NoSocNet.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{
    public class EFCoreUserRepository : EFCoreRepositoryBase<UserEntity, string>, IUserRepository
    {
        public EFCoreUserRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<UserEntity>> GetNonParticipantsForRoomAsync(string userId, string keywords = null, int take = 10, int skip = 0)
        {
            var query = this.context.Set<UserEntity>()
              .Where(x => x.Id != userId && x.UserRooms.Where(ur => ur.ChatRoom.IsPrivate).All(ur => !ur.ChatRoom.UserRooms.Any(r => r.UserId == userId)));


            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = query.Where(x => x.UserName.Contains(keywords));
            }

            var data = await query.Skip(skip).Take(take).ToListAsync();
            return data;
        }

        public async override Task<ICollection<UserEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
        {
            if (skip < 0)
            {
                throw new ArgumentException(nameof(skip));
            }

            if (take <= 0)
            {
                throw new ArgumentException(nameof(take));
            }

            if (String.IsNullOrWhiteSpace(currentUserId))
            {
                throw new ArgumentException(nameof(currentUserId));
            }

            IQueryable<UserEntity> query = this.context
                .Set<UserEntity>()
                .Where(x => x.Id != currentUserId);

            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = query.Where(x => x.UserName.Contains(keywords) || x.Email.Contains(keywords));
            }

            return await query.Skip(skip).Take(take).ToListAsync();
        }
    }
}
