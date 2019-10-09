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
    public class RoomRepository : IChatRoomRepository<User, string>
    {
        private readonly DbContext context;
        private DbSet<ChatRoomEntity> ChatRooms => context.Set<ChatRoomEntity>();
        private DbSet<MessageEntity> Messages => context.Set<MessageEntity>();

        public RoomRepository(DbContext context)
        {
            this.context = context;
        }

        public Task CreatePrivateRoom(string firstUserId, string secondUserId)
        {
            ChatRoomEntity newRoom = new ChatRoomEntity { IsPrivate = true, OwnerUserId = firstUserId };

            newRoom.UserRooms = new List<UsersChatRoomsEntity> {
                    new UsersChatRoomsEntity {
                        ChatRoom =newRoom,
                        UserId=firstUserId
                    },
                    new UsersChatRoomsEntity {
                        ChatRoom=newRoom,
                        UserId=secondUserId
                    }
                };

            this.context.Add(newRoom);

            return Task.CompletedTask;
        }

        public async Task<ChatRoom<User, string>> GetPrivateRoom(string firstUserId, string secondUserId)
        {
            var query = this.ChatRooms
                .Include(x => x.OwnerUser)
                .Include(x => x.UserRooms).ThenInclude(x => x.User)
                //should i explicitly load messages from MessagesRepo ?
                .Include(x => x.Messages).ThenInclude(x => x.SenderUser)
                .Where(x => x.IsPrivate && x.UserRooms.Any(ur => ur.UserId == firstUserId) && x.UserRooms.Any(ur => ur.UserId == secondUserId));

            if (await query.FirstOrDefaultAsync() is ChatRoomEntity privateRoom)
            {
                return new ChatRoom<User, string>
                {
                    Id = privateRoom.Id,
                    IsPrivate = privateRoom.IsPrivate,
                    Owner = privateRoom.OwnerUser,
                    OwnerId = privateRoom.OwnerUserId,
                    Messages = privateRoom.Messages.Select(x => new Message<User, string>
                    {
                        ChatRoomId = privateRoom.Id,
                        Id = x.Id,
                        SendDate = x.SendDate,
                        SenderId = x.SenderUserId,
                        Sender = x.SenderUser,
                        Text = x.Text
                    }),
                    Participants = privateRoom.UserRooms.Select(x => x.User)
                };
            }

            return null;
        }

        private IQueryable<ChatRoomEntity> ApplyPagination(IQueryable<ChatRoomEntity> query, Paginator paginator)
        {
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

            if (!String.IsNullOrWhiteSpace(paginator?.TailId))
            {

                var ids = query.Select(x => x.Id).ToList();
                int skip = ids.IndexOf(paginator.TailId);
                skip = skip > 0 ? skip + 1 : 0;
                query = query.Skip(skip)
                    .Take(paginator.PageSize);
            }
            else
            {
                query = query
                    .Skip((paginator.Page - 1) * paginator.PageSize)
                    .Take(paginator.PageSize);
            }

            return query;
        }
        private IQueryable<ChatRoomEntity> ApplyFilter(IQueryable<ChatRoomEntity> query, FilterBase filter)
        {
            if (!String.IsNullOrEmpty(filter?.Keywords))
            {
                query = query.Where(x => x.RoomName.Contains(filter.Keywords) || x.UserRooms.Any(ur => ur.User.UserName.Contains(filter.Keywords)));
            }
            return query;
        }

        public async Task<PagedList<ChatRoom<User, string>>> GetRoomsAsync(string userId, FilterBase filter = null, Paginator pagination = default)
        {
            if (pagination == null)
            {
                pagination = new Paginator
                {
                    Page = 1,
                    PageSize = 10,
                    SortColumn = "",
                    SortOrder = SortOrder.Descending
                };
            }

            var query = ChatRooms
                .Where(x => x.UserRooms.Any(u => u.UserId == userId));

            query = query
                .Include(x => x.OwnerUser)
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

            var selectedChatRoomIds = data.Select(x => x.ChatRoom.Id).ToArray();

            var messageLookup = Messages
                .Include(x => x.SenderUser)
                .Include(x => x.ReadByUsers).ThenInclude(x => x.User)
                .Where(x => selectedChatRoomIds.Contains(x.ChatRoomId))
                .GroupBy(x => x.ChatRoomId)
                .SelectMany(x => x.OrderByDescending(m => m.SendDate).Take(10))
                .ToLookup(x => x.ChatRoomId);

            foreach (var item in data)
            {
                item.ChatRoom.Messages = messageLookup[item.ChatRoom.Id].Reverse().ToList();
            }

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

        //public async Task<PagedList<ChatRoom<User, string>>> GetRoomsAsync(string userId, int size, string tailRoomId = null)
        //{
        //    var query = context
        //        .Set<ChatRoomEntity>()
        //        .Where(x => x.UserRooms.Any(u => u.UserId == userId));

        //    query = query
        //        .Include(x => x.OwnerUser)
        //        .Include(x => x.Messages).ThenInclude(x => x.SenderUser).Take(2)
        //        .Include(x => x.UserRooms).ThenInclude(x => x.User);

        //    int count = await query.CountAsync();
        //    int skip = 0;

        //    if (!String.IsNullOrWhiteSpace(tailRoomId))
        //    {
        //        var ids = await query.Select(x => x.Id).ToListAsync();
        //        skip = ids.IndexOf(tailRoomId);
        //        skip = skip > 0 ? skip + 1 : 0;
        //    }

        //    var data = await query
        //        .Select(x => new
        //        {
        //            ChatRoom = x,
        //            HasUnread = x.Messages.Any(m => m.ReadByUsers.All(r => r.UserId != userId))
        //        })
        //        .Skip(skip)
        //        .Take(size)
        //        .ToListAsync();

        //    var items = data
        //        .Select(x => new ChatRoom<User, string>
        //        {
        //            //ToDo: Use automapper
        //            Id = x.ChatRoom.Id,
        //            IsPrivate = x.ChatRoom.IsPrivate,
        //            OwnerId = x.ChatRoom.OwnerUserId,
        //            Owner = x.ChatRoom.OwnerUser,
        //            RoomName = x.ChatRoom.RoomName,
        //            HasUnread = x.HasUnread,
        //            Participants = x.ChatRoom.UserRooms.Select(bind => bind.User).ToList(),
        //            Messages = x.ChatRoom.Messages.Select(msg => new Message<User, string>()
        //            {
        //                ChatRoomId = msg.ChatRoomId,
        //                Id = msg.Id,
        //                SendDate = msg.SendDate,
        //                SenderId = msg.SenderUserId,
        //                Sender = msg.SenderUser,
        //                ChatRoom = null,
        //                Text = msg.Text
        //            })
        //        });

        //    return new PagedList<ChatRoom<User, string>>(items, count);
        //}

        public async Task<ChatRoom<User, string>> GetRoom(string roomId)
        {
            var room = await ChatRooms
                .Include(x => x.Messages).ThenInclude(x => x.SenderUser)
                .Include(x => x.OwnerUser)
                .Include(x => x.UserRooms).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == roomId);

            if (room == null)
            {
                return null;
            }

            return new ChatRoom<User, string>
            {
                Id = room.Id,
                IsPrivate = room.IsPrivate,
                OwnerId = room.OwnerUserId,
                Owner = room.OwnerUser,
                RoomName = room.RoomName,
                Participants = room.UserRooms.Select(bind => bind.User).ToList(),
                Messages = room.Messages.Select(msg => new Message<User, string>()
                {
                    ChatRoomId = msg.ChatRoomId,
                    Id = msg.Id,
                    SendDate = msg.SendDate,
                    SenderId = msg.SenderUserId,
                    Sender = msg.SenderUser,
                    ChatRoom = null,
                    Text = msg.Text
                })
            };
        }

        public async Task AddUserToRoom(string userId, string roomId)
        {
            if (await ChatRooms.FindAsync(roomId) is ChatRoomEntity room)
            {
                if (room.UserRooms.All(x => x.UserId != userId))
                {
                    room.IsPrivate = false;
                    room.UserRooms.Add(new UsersChatRoomsEntity
                    {
                        UserId = userId,
                        ChatRoomId = roomId
                    });

                    this.context.Update(room);
                }
            }
        }
    }
}
