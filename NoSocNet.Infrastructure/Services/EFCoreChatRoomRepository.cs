using Microsoft.EntityFrameworkCore;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Interfaces.Repositories;
using NoSocNet.Domain.Models;
using NoSocNet.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NoSocNet.Infrastructure.Services
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

        public async Task AddParticipantsAsync(string chatRoomId, IEnumerable<string> usersIds)
        {
            var bind = usersIds.Select(x => new UsersChatRoomsEntity
            {
                ChatRoomId = chatRoomId.ToLower(),
                UserId = x
            });

            ChatRoomEntity room = await context.Set<ChatRoomEntity>().FindAsync(chatRoomId);
            room.IsPrivate = false;
            context.MarkCreate(bind);
            context.MarkUpdate(room);
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatRoomEntity>> GetRecentChatRoomsAsync(string userId, string[] skipIds, int count = 10)
        {
            string exclude = null;
            if (skipIds != null && skipIds.Count() > 0)
            {
                exclude = $"AND CR.Id NOT IN({skipIds.Aggregate("", (str, guid) => str += $"N'{guid.ToLower()}',").Trim(',')})";
            }

            List<ChatRoomEntity> items = await this.context.ChatRooms.FromSql("SELECT TOP({0}) * FROM [ChatRooms] AS CR" +
               " left join [UsersRooms] as UR on UR.ChatRoomId = CR.Id " +
               " where UR.UserId = {1} "
               + (exclude ?? "") +
                 "ORDER BY(SELECT TOP(1) SendDate FROM Messages as M WHERE M.ChatRoomId = CR.Id ORDER BY M.Id DESC) DESC", count, userId)
               //.Include(x => x.UserRooms).ThenInclude(x => x.User)   //results in re-ordering 
               .Include(x => x.OwnerUser)
               .ToListAsync();

            //var wtfdata = await this.context.ChatRooms.FromSql("recentChats @p0 , @p1 , @p2", count, userId, exclude).ToListAsync();


            var ids = items.Select(x => x.Id).ToArray();

            //shoul be removed

            ILookup<string, UsersChatRoomsEntity> usersLookup = (await this.context.Set<UsersChatRoomsEntity>()
                .Include(x => x.User)
                .Where(x => ids.Contains(x.ChatRoomId))
                .ToArrayAsync())
                .ToLookup(x => x.ChatRoomId, x => x);

            ILookup<string, MessageEntity> messageLookup = (await this.context.Set<MessageEntity>()
                .Where(x => ids.Contains(x.ChatRoomId))
                .GroupBy(x => x.ChatRoomId)
                .SelectMany(x => x.OrderByDescending(m => m.SendDate).Take(10))
                .ToArrayAsync())
                .ToLookup(x => x.ChatRoomId, x => x);


            foreach (var item in items)
            {
                item.Messages = messageLookup[item.Id].Reverse().ToList();
                item.UserRooms = usersLookup[item.Id].ToList();
            }

            return items;
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
                    HasUnread = x.Messages.Any(m => m.SenderUserId != userId && m.ReadByUsers.All(r => r.UserId != userId))
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

