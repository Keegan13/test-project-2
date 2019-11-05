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
    public class EFCoreChatRoomRepository : EFCoreRepositoryBase<ChatRoomEntity, string>, IChatRoomRepository
    {

        public EFCoreChatRoomRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<ChatRoomEntity> GetPrivateRoomAsync(string firstUserId, string secondUserId)
        {
            var query = this.context.Set<ChatRoomEntity>()
               .Include(x => x.OwnerUser)
               .Include(x => x.UserRooms).ThenInclude(x => x.User)
               //should i explicitly load messages from MessagesRepo ?
               .Include(x => x.Messages).ThenInclude(x => x.SenderUser)
               .Where(x => x.IsPrivate && x.UserRooms.Any(ur => ur.UserId == firstUserId) && x.UserRooms.Any(ur => ur.UserId == secondUserId));

            return query.FirstOrDefaultAsync();
        }

        public async override Task<ChatRoomEntity> FindByIdAsync(string keys)
        {
            var room = await this.context.Set<ChatRoomEntity>().Include(x => x.UserRooms).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == keys);

            if (room != null)
            {
                await this.context.Entry(room).Collection(x => x.Messages).Query().OrderByDescending(x => x.SendDate).Take(10).LoadAsync();

            }

            return room;
        }

        public Task CreatePrivateRoomAsync(string firstUserId, string secondUserId)
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

        public Task AddParticipantsAsync(string chatRoomId, IEnumerable<string> usersIds)
        {
            var bind = usersIds.Select(x => new UsersChatRoomsEntity
            {
                ChatRoomId = chatRoomId,
                UserId = x
            });

            context.MarkCreate(bind);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ChatRoomEntity>> GetRecentChatRoomsAsync(string userId, string[] skipIds, int count = 10)
        {
            IQueryable<ChatRoomEntity> query = this.context.Set<ChatRoomEntity>()
                .Where(x => x.UserRooms.Any(u => u.UserId == userId));

            query = query
                .Include(x => x.OwnerUser)
                .Include(x => x.UserRooms).ThenInclude(x => x.User);
            //.Include(x => x.Messages).ThenInclude(x => x.SenderUser);


            if (skipIds != null && skipIds.Length > 0)
            {
                query = query.Where(x => !skipIds.Contains(x.Id));
            }


            query = query.OrderBy(x => x.Messages.OrderByDescending(m => m.SendDate).Select(m => m.SendDate).FirstOrDefault()).Take(10);

            var data = await query
                .ToListAsync();

            var selectedChatRoomIds = data.Select(x => x.Id).ToArray();




            var messageLookup = (from message in this.context.Set<MessageEntity>()
                                 where selectedChatRoomIds.Contains(message.ChatRoomId)
                                 group message by message.ChatRoomId into messageGroup
                                 select messageGroup)
                            .SelectMany(x => x.OrderByDescending(m => m.SendDate).Take(10)).ToLookup((x) => x.ChatRoomId);

            foreach (var item in data)
            {
                item.Messages = messageLookup[item.Id].Reverse().ToList();
            }

            return data;
        }

        public async Task<IEnumerable<ChatRoomEntity>> SearchRoomsAsync(string userId, string keywords, int take = 10, int skip = 0)
        {
            IQueryable<ChatRoomEntity> query = this.context
                .Set<ChatRoomEntity>()
                .Where(x => x.UserRooms.Any(ur => ur.UserId == userId));

            if (!String.IsNullOrEmpty(keywords))
            {
                query = query.Where(x => x.RoomName.Contains(keywords) || x.UserRooms.Any(ur => ur.User.UserName.Contains(keywords)));
            }

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IDictionary<string, bool>> GetHasUnreadAsync(string[] chatRoomsIds, string userId)
        {
            return (await this.context.Set<ChatRoomEntity>()
                .Where(x => chatRoomsIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    HasUnread = x.Messages.Any(m => m.ReadByUsers.All(r => r.UserId != userId))
                })
                .ToArrayAsync())
                .ToDictionary(x => x.Id, x => x.HasUnread);
        }

        public override async Task<ICollection<ChatRoomEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10)
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

            IQueryable<ChatRoomEntity> query = this.context
                .Set<ChatRoomEntity>()
                .Include(x => x.UserRooms).ThenInclude(x => x.User)
                .Where(x => x.UserRooms.Any(ur => ur.UserId == currentUserId));

            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = query.Where(x => x.RoomName.Contains(keywords) || x.UserRooms.Any(ur => ur.User.UserName.Contains(keywords) || ur.User.Email.Contains(keywords)));
            }

            var data = await query.Skip(skip).Take(take).ToListAsync();

            return data;
        }
    }
}

