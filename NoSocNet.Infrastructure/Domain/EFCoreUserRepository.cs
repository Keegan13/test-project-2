using Microsoft.EntityFrameworkCore;
using NoSocNet.Domain.Interfaces;
using NoSocNet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Domain
{
    public class EFCoreUserRepository : EFCoreRepositoryBase<UserEntity, string>, IUserRepository
    {
        public EFCoreUserRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<NoSocNet.Domain.Models.UserEntity>> GetNonParticipantsForRoomAsync(string userId, string keywords = null, int take = 10, int skip = 0)
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

        async Task<NoSocNet.Domain.Models.UserEntity> IUserRepository.FindByIdAsync(string id)
        {
            return await base.FindByIdAsync(id);
        }
    }
}
