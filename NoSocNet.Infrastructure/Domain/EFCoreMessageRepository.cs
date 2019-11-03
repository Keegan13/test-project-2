using Microsoft.EntityFrameworkCore;
using NoSocNet.Domain.Interfaces;
using NoSocNet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Domain
{
    public class EFCoreMessageRepository : EFCoreRepositoryBase<MessageEntity, int>, IMessageRepository
    {
        public EFCoreMessageRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<MessageEntity>> GetMessagesAsync(string chatRoomId, int count, int? tailMessageId = null)
        {
            if (String.IsNullOrEmpty(chatRoomId))
            {
                throw new ArgumentNullException(nameof(chatRoomId));
            }

            if (tailMessageId.HasValue && tailMessageId.Value <= 0)
            {
                throw new ArgumentException(nameof(tailMessageId));
            }

            if (count < 0)
            {
                throw new ArgumentException(nameof(count));
            }

            if (count == 0)
            {
                return Enumerable.Empty<MessageEntity>();
            }

            IQueryable<MessageEntity> query = context.Set<MessageEntity>()
                .Include(x => x.SenderUser)
                .Include(x => x.ReadByUsers);

            if (tailMessageId.HasValue)
            {
                query = query.Where(x => x.ChatRoomId == chatRoomId && x.Id < tailMessageId.Value);
            }

            List<MessageEntity> data = await query.OrderByDescending(x => x.SendDate)
                  .Take(count)
                  .ToListAsync();

            return data.OrderBy(x => x.SendDate);
        }

        public async Task SetReadByUserAsync(string userId, string chatRoomId, int? tillMessageId = null)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (String.IsNullOrEmpty(chatRoomId))
            {
                throw new ArgumentNullException(nameof(chatRoomId));
            }

            var query = context.Messages
                .Where(x => x.ChatRoomId == chatRoomId && x.ReadByUsers.All(rb => rb.UserId != userId));

            if (tillMessageId.HasValue)
            {
                query = query.Where(x => x.Id <= tillMessageId.Value);
            }

            var readMessages = await query.Select(x => x.Id).ToListAsync();

            var reads = readMessages
                .Select(id => new MessageReadByUserEntity
                {
                    MessageId = id,
                    UserId = userId
                })
                .ToArray();

            await context.Set<MessageReadByUserEntity>().AddRangeAsync(reads);

            await unitOfWork.SaveChangesAsync();
        }
    }
}
