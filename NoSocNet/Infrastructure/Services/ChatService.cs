using AutoMapper.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Models;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NoSocNet.Infrastructure.Services
{
    public class ChatService : IChatService<User, string>, IRoomStore<User, string>
    {
        private readonly DbContext context;
        private readonly IIdentityService<User> identityService;
        private readonly IHubSender<User, string> messageSender;

        protected virtual IQueryable<MessageDto> MessagesQuery => context
            .Set<MessageDto>()
            .Include(x => x.ChatRoom).ThenInclude(x => x.UserRooms)
            .Include(x => x.Sender)
            ;
        protected virtual IQueryable<UsersChatRoomsDto> UserRoomQuery => context
            .Set<UsersChatRoomsDto>();

        protected virtual IQueryable<ChatRoomDto> RoomsQuery => context
            .Set<ChatRoomDto>()
            .Include(x => x.Owner)
            .Include(x => x.Messages).ThenInclude(x => x.Sender)
            .Include(x => x.UserRooms).ThenInclude(x => x.User);

        public ChatService(DbContext context, IIdentityService<User> identity, IHubSender<User, string> messageSender)
        {
            this.context = context;
            this.messageSender = messageSender;
            this.identityService = identity;
        }

        public async Task<IList<Message<User, string>>> GetChatMessages(string chatRoomId)
        {
            return await MessagesQuery
                .Where(x => x.ChatRoomId == chatRoomId)
                .OrderByDescending(x => x.SendDate)
                .Select(x => new Message<User, string>
                {
                    //ToDo: use AutoMapper
                    Id = x.Id,
                    ChatRoomId = x.ChatRoomId,
                    SendDate = x.SendDate,
                    SenderId = x.SenderId,
                    Text = x.Text,

                    Sender = x.Sender,
                    ChatRoom = new ChatRoom<User, string>
                    {
                        IsPrivate = x.ChatRoom.IsPrivate,
                        OwnerId = x.ChatRoom.OwnerId,
                        Id = x.ChatRoom.Id,
                        Participants = x.ChatRoom.UserRooms.Select(bind => bind.User)
                    }
                }).ToListAsync();
        }

        public async Task<IList<ChatRoom<User, string>>> GetUserRooms(string userId)
        {
            return await this.RoomsQuery
                .Where(x => x.UserRooms.Any(u => u.UserId == userId))
                .Select(x => new ChatRoom<User, string>
                {
                    //ToDo: Use automapper
                    Id = x.Id,
                    IsPrivate = x.IsPrivate,
                    OwnerId = x.OwnerId,
                    Owner = x.Owner,
                    Participants = x.UserRooms.Select(bind => bind.User).ToList(),
                    Messages = x.Messages.Select(msg => new Message<User, string>()
                    {
                        ChatRoomId = msg.ChatRoomId,
                        Id = msg.Id,
                        SendDate = msg.SendDate,
                        SenderId = msg.SenderId,
                        Sender = msg.Sender,
                        ChatRoom = null,
                        Text = msg.Text
                    })
                }).ToListAsync();
        }

        public async Task<ChatRoom<User, string>> JoinPrivateAsync(string userId)
        {
            string[] users = new[] { userId, identityService.CurrentUserId };

            var privateRoom = await this.RoomsQuery
                .FirstOrDefaultAsync(x => x.IsPrivate && x.UserRooms.Any(ur => users.Contains(ur.UserId)));

            if (privateRoom == null)
            {
                ChatRoomDto newRoom = new ChatRoomDto { IsPrivate = true };

                newRoom.UserRooms = new List<UsersChatRoomsDto> {
                    new UsersChatRoomsDto {
                        ChatRoom =newRoom,
                        UserId=userId
                    },
                    new UsersChatRoomsDto {
                        ChatRoom=newRoom,
                        UserId=identityService.CurrentUserId
                    }
                };

                this.context.Add(newRoom);

                await this.context.SaveChangesAsync();

                privateRoom = newRoom;
            }

            return new ChatRoom<User, string>
            {
                Id = privateRoom.Id,
                IsPrivate = privateRoom.IsPrivate,
                Owner = privateRoom.Owner,
                OwnerId = privateRoom.OwnerId,
                Messages = privateRoom.Messages.Select(x => new Message<User, string>
                {
                    ChatRoomId = privateRoom.Id,
                    Id = x.Id,
                    SendDate = x.SendDate,
                    SenderId = x.SenderId,
                    Sender = x.Sender,
                    Text = x.Text
                }),
                Participants = privateRoom.UserRooms.Select(x => x.User)
            };
        }

        public async Task<Message<User, string>> Push(string userId, string roomId, string text)
        {
            MessageDto newMessage = new MessageDto
            {
                ChatRoomId = roomId,
                SendDate = DateTime.Now,
                SenderId = identityService.CurrentUserId,
                Text = text
            };

            context.Set<MessageDto>().Add(newMessage);

            await context.SaveChangesAsync();

            await context.Entry(newMessage).Reference(s => s.Sender).LoadAsync();
            await context.Entry(newMessage)
                .Reference(s => s.ChatRoom)
                .Query()
                .Include(x => x.UserRooms)
                .ThenInclude(x => x.User)
                .LoadAsync();

            var message = new Message<User, string>()
            {
                //ToDo: move to AutoMapper
                ChatRoomId = newMessage.ChatRoomId,
                SendDate = newMessage.SendDate,
                Text = newMessage.Text,
                SenderId = newMessage.SenderId,
                Sender = newMessage.Sender,
                ChatRoom = new ChatRoom<User, string>
                {
                    Id = newMessage.ChatRoomId,
                    Participants = newMessage.ChatRoom.UserRooms.Select(x => x.User)
                },
                Id = newMessage.Id
            };

            await this.messageSender.PushMessage(message, roomId);

            return message;
        }

        public async Task<ChatRoom<User, string>> InviteToRoom(string userId, string roomId)
        {
            var room = await this.RoomsQuery
                .FirstOrDefaultAsync(x => x.Id == roomId);

            if (room != null && !room.IsPrivate)
            {
                return new ChatRoom<User, string>
                {
                    Id = room.Id,
                    IsPrivate = room.IsPrivate,
                    Owner = room.Owner,
                    OwnerId = room.OwnerId,
                    Messages = room.Messages.Select(x => new Message<User, string>
                    {
                        ChatRoomId = room.Id,
                        Id = x.Id,
                        SendDate = x.SendDate,
                        SenderId = x.SenderId,
                        Sender = x.Sender,
                        Text = x.Text
                    }),
                    Participants = room.UserRooms.Select(x => x.User)
                };
            }

            return null;
        }

        public async Task<ChatRoom<User, string>> GetRoomAsync(string roomId)
        {
            if (String.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId));
            }

            var item = await this.RoomsQuery.FirstOrDefaultAsync();


            return item != null ? new ChatRoom<User, string>
            {
                Id = item.Id,
                IsPrivate = item.IsPrivate,
                Owner = item.Owner,
                OwnerId = item.OwnerId,
                Messages = item.Messages.Select(x => new Message<User, string>
                {
                    ChatRoomId = x.ChatRoomId,
                    Id = x.Id,
                    SendDate = x.SendDate,
                    SenderId = x.SenderId,
                    Text = x.Text,
                    Sender = x.Sender
                }),
                Participants = item.UserRooms.Select(x => x.User)
            } : null;
        }
    }
}
