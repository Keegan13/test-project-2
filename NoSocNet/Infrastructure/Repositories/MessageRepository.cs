using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.BLL.Models;
using NoSocNet.DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository<User, string>
    {
        private readonly DbContext context;
        private readonly IUnitOfWork unitOfWork;
        private DbSet<MessageEntity> Messages => context.Set<MessageEntity>();

        public MessageRepository(IUnitOfWork uow,
            DbContext context
            )
        {
            this.unitOfWork = uow;
            this.context = context;
        }

        public async Task<PagedList<Message<User, string>>> GetMessagesAsync(string chatRoomId, FilterBase filter = null, Paginator paginator = null)
        {
            var query = context
                .Set<MessageEntity>()
                .Include(x => x.ChatRoom)
                .Include(x => x.SenderUser)
                .Include(x => x.ReadByUsers)
                .Where(x => x.ChatRoomId == chatRoomId);

            query = ApplyFilter(query, filter);
            int count = await query.CountAsync();

            query = ApplySorting(query, paginator);
            query = ApplyPagination(query, paginator);

            var data = await query.ToListAsync();

            var items = query.Select(x => new Message<User, string>
            {
                Id = x.Id,
                ChatRoom = new ChatRoom<User, string>
                {
                    Id = x.ChatRoom.Id,
                    IsPrivate = x.ChatRoom.IsPrivate,
                    OwnerId = x.ChatRoom.OwnerUserId,
                    Owner = x.ChatRoom.OwnerUser,
                    RoomName = x.ChatRoom.RoomName
                },
                SendDate = x.SendDate,
                Sender = x.SenderUser,
                ChatRoomId = x.ChatRoomId,
                ReadByUsers = x.ReadByUsers.Select(r => r.User).ToList(),
                Text = x.Text,
                SenderId = x.SenderUserId
            });

            return new PagedList<Message<User, string>>(items, count, filter, paginator);
        }
        protected virtual IQueryable<MessageEntity> ApplySorting(IQueryable<MessageEntity> query, Paginator paginator)
        {
            if (String.IsNullOrEmpty(paginator?.SortColumn))
            {
                return paginator.SortOrder == SortOrder.Ascending ? query.OrderBy(x => x.SendDate) : query.OrderByDescending(x => x.SendDate);
            }

            return query;
        }

        protected virtual IQueryable<MessageEntity> ApplyPagination(IQueryable<MessageEntity> query, Paginator paginator)
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

            if (!String.IsNullOrEmpty(paginator.TailId) && int.TryParse(paginator.TailId, out int msgId) && msgId > 0)
            {
                //ToDo implement tailing
            }
            else
            {
                query = query
                        .Skip((paginator.Page - 1) * paginator.PageSize)
                        .Take(paginator.PageSize);
            }

            return query;
        }

        protected virtual IQueryable<MessageEntity> ApplyFilter(IQueryable<MessageEntity> query, FilterBase filter)
        {
            if (String.IsNullOrEmpty(filter?.Keywords))
            {
                return query.Where(x => x.Text.Contains(filter.Keywords));
            }

            return query;
        }

        public async Task<IEnumerable<Message<User, string>>> GetMessagesAsync(string roomId, int size, int? tailId = null)
        {
            if (String.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId));
            }

            if (size <= 0)
            {
                return Enumerable.Empty<Message<User, string>>();
            }

            if (!tailId.HasValue || tailId.Value <= 0)
            {
                return Enumerable.Empty<Message<User, string>>();
            }

            var data = await context.Set<MessageEntity>()
                .Include(x => x.SenderUser)
                .Include(x => x.ReadByUsers)
                .Where(x => x.ChatRoomId == roomId && x.Id > tailId)
                .OrderByDescending(x => x.SendDate)
                .Take(size)
                .Select(x => new Message<User, string>
                {
                    Id = x.Id,
                    SendDate = x.SendDate,
                    Text = x.Text,
                    Sender = x.SenderUser,
                    SenderId = x.SenderUserId,
                    ChatRoomId = x.ChatRoomId,
                    ReadByUsers = x.ReadByUsers.Select(ru => ru.User)
                })
                .ToListAsync();


            return data;
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

            var query = context.Set<MessageEntity>().Where(x => x.ChatRoomId == roomId && x.ReadByUsers.All(rb => rb.UserId != userId));

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

            await unitOfWork.SaveAsync();
            //await context.SaveChangesAsync();
        }

        public async Task AddMessage(Message<User, string> message)
        {
            MessageEntity newMessage = new MessageEntity
            {
                ChatRoomId = message.ChatRoomId,
                SendDate = DateTime.Now,
                SenderUserId = message.SenderId,
                Text = message.Text
            };

            newMessage.ReadByUsers = new List<MessageReadByUserEntity> {
                new MessageReadByUserEntity {
                    Message=newMessage,
                    UserId=message.SenderId
            }};

            Messages.Add(newMessage);
            await unitOfWork.SaveAsync();

            await context.Entry(newMessage).Reference(s => s.SenderUser).LoadAsync();
            await context.Entry(newMessage)
                .Reference(s => s.ChatRoom)
                .Query()
                .Include(x => x.UserRooms)
                .ThenInclude(x => x.User)
                .LoadAsync();

            message.Sender = newMessage.SenderUser;
            message.SendDate = newMessage.SendDate;
            message.Id = newMessage.Id;
            message.ReadByUsers = newMessage.ReadByUsers.Select(x => x.User);
            message.ChatRoom = new ChatRoom<User, string>
            {
                Id = newMessage.ChatRoom.Id,
                RoomName = newMessage.ChatRoom.RoomName,
                OwnerId = newMessage.ChatRoom.OwnerUserId,
                Participants = newMessage.ChatRoom.UserRooms.Select(x => x.User),
                IsPrivate = newMessage.ChatRoom.IsPrivate
            };
        }
    }
}
