using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository<string>
    {
        private readonly DbContext context;
        public MessageRepository(
            DbContext context
            )
        {
            this.context = context;
        }

        public async Task SetReadByUserAsync(string userId, string roomId, int? tillMessageId = null)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (String.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId));
            }

            var query = context.Set<MessageDto>().Where(x => x.ChatRoomId == roomId && x.ReadByUsers.All(rb => rb.UserId != userId));

            if (tillMessageId.HasValue)
            {
                query = query.Where(x => x.Id <= tillMessageId.Value);
            }

            var readMessages = await query.Select(x => x.Id).ToListAsync();
            var reads = readMessages.Select(id => new MessageReadByUserDto
            {
                MessageId = id,
                UserId = userId
            }).ToArray();

            await context.Set<MessageReadByUserDto>().AddRangeAsync(reads);
            await context.SaveChangesAsync();
        }
    }
}
