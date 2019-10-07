using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.BLL.Models;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepositoryStore<User, string>
    {
        private readonly DbContext context;

        public RoomRepository(DbContext context)
        {
            this.context = context;
        }

        public Task<ChatRoom<User, string>> CreatePrivateRoom(string firstUserId, string secondUserId)
        {
            throw new NotImplementedException();
        }

        public Task<ChatRoom<User, string>> GetPrivateRoom(string firstUserId, string secondUserId)
        {
            throw new NotImplementedException();
        }

        private IQueryable<ChatRoomDto> ApplyPagination(IQueryable<ChatRoomDto> query, Paginator paginator)
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


            if (String.Compare(paginator.SortColumn, "Messages", true) == 0)
            {
                if (paginator.SortOrder == SortOrder.Ascending)
                {
                    query = query.OrderBy(x => x.Messages.OrderByDescending(m => m.SendDate).Select(m => m.SendDate).FirstOrDefault());

                }
                else
                {
                    query = query.OrderByDescending(x => x.Messages.OrderByDescending(m => m.SendDate).Select(m => m.SendDate).FirstOrDefault());
                }
            }

            //ToDo: OrderByColumnName extesniosn method

            query = query
                    .Skip((paginator.Page - 1) * paginator.PageSize)
                    .Take(paginator.PageSize);

            return query;
        }
        private IQueryable<ChatRoomDto> ApplyFilter(IQueryable<ChatRoomDto> query, FilterBase filter)
        {
            if (!String.IsNullOrEmpty(filter?.Keywords))
            {
                query = query.Where(x => x.RoomName.Contains(filter.Keywords));
            }
            return query;
        }

        public async Task<PagedList<ChatRoom<User, string>>> GetRoomsByUserAsync(string userId, FilterBase filter = null, Paginator pagination = default)
        {
            var query = context
                .Set<ChatRoomDto>()
                .Where(x => x.UserRooms.Any(u => u.UserId == userId));

            query = query
                .Include(x => x.OwnerUser)
                .Include(x => x.Messages).ThenInclude(x => x.SenderUser)
                .Include(x => x.UserRooms).ThenInclude(x => x.User);

            query = ApplyFilter(query, filter);
            int count = await query.CountAsync();

            query = ApplyPagination(query, pagination);

            var data = await query
                .Select(x => new
                {
                    ChatRoom = x,
                    HasUnread = x.Messages.Any(m => m.ReadByUsers.All(r => r.UserId != userId))
                })
                .ToListAsync();

            var items = data
                .Select(x => new ChatRoom<User, string>
                {
                    //ToDo: Use automapper
                    Id = x.ChatRoom.Id,
                    IsPrivate = x.ChatRoom.IsPrivate,
                    OwnerId = x.ChatRoom.OwnerUserId,
                    Owner = x.ChatRoom.OwnerUser,
                    RoomName = x.ChatRoom.RoomName,
                    HasUnread = x.HasUnread,
                    Participants = x.ChatRoom.UserRooms.Select(bind => bind.User).ToList(),
                    Messages = x.ChatRoom.Messages.Select(msg => new Message<User, string>()
                    {
                        ChatRoomId = msg.ChatRoomId,
                        Id = msg.Id,
                        SendDate = msg.SendDate,
                        SenderId = msg.SenderUserId,
                        Sender = msg.SenderUser,
                        ChatRoom = null,
                        Text = msg.Text
                    })
                });

            return new PagedList<ChatRoom<User, string>>(items, count, filter, pagination);
        }
    }
}
