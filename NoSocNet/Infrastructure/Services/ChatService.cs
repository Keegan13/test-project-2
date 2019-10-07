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
using ExpressionBuilder;
using ExpressionBuilder.Generics;
using ExpressionBuilder.Operations;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.Infrastructure.Repositories;

namespace NoSocNet.Infrastructure.Services
{
    public class ChatService : IChatService<User, string>, IRoomStore<User, string>
    {
        private readonly DbContext context;
        private readonly IIdentityService<User> identityService;
        private readonly IApplicationUserStore<User> userStore;
        private readonly INotificator<string> notificator;
        private readonly IRoomRepositoryStore<User, string> roomRepo;
        private readonly IUserRepository<User, string> userRepo;
        private readonly IMessageRepository<string> messageRepo;

        public ChatService(
            DbContext context,
            IIdentityService<User> identity,
            INotificator<string> notificator,
            IApplicationUserStore<User> userStore,
            IRoomRepositoryStore<User, string> roomRepo,
            IUserRepository<User, string> userRepo,
            IMessageRepository<string> messageRepo
            )
        {
            this.messageRepo = messageRepo;
            this.userRepo = userRepo;
            this.roomRepo = roomRepo;
            this.userStore = userStore;
            this.context = context;
            this.notificator = notificator;
            this.identityService = identity;
        }

        protected virtual IQueryable<MessageDto> MessagesQuery => context
            .Set<MessageDto>()
            .Include(x => x.ChatRoom).ThenInclude(x => x.UserRooms)
            .Include(x => x.SenderUser)
            ;
        protected virtual IQueryable<UsersChatRoomsDto> UserRoomQuery => context
            .Set<UsersChatRoomsDto>();

        protected virtual IQueryable<ChatRoomDto> RoomsQuery => context
            .Set<ChatRoomDto>()
            .Include(x => x.OwnerUser)
            .Include(x => x.Messages).ThenInclude(x => x.SenderUser)
            .Include(x => x.UserRooms).ThenInclude(x => x.User);

        public async Task<IList<ChatRoom<User, string>>> GetRoomsByUserAsync(string userId)
        {
            //ToDo: temp solution
            DateTime reallyOld = DateTime.Parse("1900-01-01");
            string currUserId = identityService.CurrentUserId;

            return (await this.RoomsQuery

                .Where(x => x.UserRooms.Any(u => u.UserId == userId))
                .Select(x => new
                {
                    ChatRoom = x,
                    HasUnread = x.Messages.Any(m => m.ReadByUsers.All(r => r.UserId != currUserId))
                })
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
                })
                .ToListAsync())
                .OrderBy(x => !x.IsPrivate)
                .ThenByDescending(x => x.Messages.LastOrDefault()?.SendDate ?? reallyOld)
                .ToList();
        }

        public async Task<ChatRoom<User, string>> GetOrCreatePrivateRoomWith(string userId)
        {
            string currentUserId = identityService.CurrentUserId;

            if (currentUserId == userId)
            {
                throw new ArgumentException("Same user error");
            }

            if (!await this.userStore.Exists(userId))
            {
                throw new ArgumentException("User does not exists");
            }



            var privateRoom = await this.RoomsQuery
                .FirstOrDefaultAsync(x => x.IsPrivate && x.UserRooms.Any(ur => ur.UserId == currentUserId) && x.UserRooms.Any(ur => ur.UserId == userId));

            if (privateRoom == null)
            {
                ChatRoomDto newRoom = new ChatRoomDto { IsPrivate = true, OwnerUserId = identityService.CurrentUserId };

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
                await this.context.Entry(newRoom).Reference(x => x.OwnerUser).LoadAsync();
                await this.context.Entry(newRoom).Collection(x => x.UserRooms).Query().Include(x => x.User).LoadAsync();
                privateRoom = newRoom;
            }

            var room = new ChatRoom<User, string>
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

            await notificator.Notificate(new NewChatNotification<User, string>(room, room.Participants.Select(x => x.Id)));

            return room;
        }

        public async Task<Message<User, string>> Push(string userId, string roomId, string text)
        {
            MessageDto newMessage = new MessageDto
            {
                ChatRoomId = roomId,
                SendDate = DateTime.Now,
                SenderUserId = identityService.CurrentUserId,
                Text = text
            };

            newMessage.ReadByUsers = new List<MessageReadByUserDto> {
                new MessageReadByUserDto {
                    Message=newMessage,
                    UserId=identityService.CurrentUserId
            }};

            context.Set<MessageDto>().Add(newMessage);

            await context.SaveChangesAsync();

            await context.Entry(newMessage).Reference(s => s.SenderUser).LoadAsync();
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
                SenderId = newMessage.SenderUserId,
                Sender = newMessage.SenderUser,
                HasUnread = false,
                ChatRoom = new ChatRoom<User, string>
                {
                    Id = newMessage.ChatRoomId,
                    Participants = newMessage.ChatRoom.UserRooms.Select(x => x.User)
                },
                Id = newMessage.Id
            };


            await this.notificator.Notificate(new MessageNotification<User, string>(message, message.ChatRoom.Participants.Select(x => x.Id)));

            return message;
        }

        public async Task<ChatRoom<User, string>> InviteToRoomAsync(string userId, string roomId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (String.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId));
            }

            if (userId == identityService.CurrentUserId)
            {
                throw new ArgumentException("User cannot invite himself to chat room", nameof(userId));
            }

            User user = await this.userStore.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception($"Cannot find user with Id {userId}");
            }

            ChatRoomDto room = await this.RoomsQuery
                .FirstOrDefaultAsync(x => x.Id == roomId);

            if (room == null)
            {
                throw new Exception($"Cannot find room with Id {roomId}");
            }

            if (room.UserRooms.All(x => x.UserId != userId))
            {
                room.IsPrivate = false;
                room.UserRooms.Add(new UsersChatRoomsDto
                {
                    User = user,
                    ChatRoom = room
                });

                this.context.Update(room);

                await this.context.SaveChangesAsync();
            }

            //ToDo: add BLL User Model
            //Temp fix
            foreach (var userRoom in room.UserRooms)
            {
                context.Entry(userRoom.User).State = EntityState.Detached;
                context.Entry(userRoom).State = EntityState.Detached;
            }

            context.Entry(room).State = EntityState.Detached;

            //end temp fix
            var roomModel = new ChatRoom<User, string>
            {
                Id = room.Id,
                IsPrivate = room.IsPrivate,
                Owner = room.OwnerUser,
                OwnerId = room.OwnerUserId,
                Messages = room.Messages.Select(x => new Message<User, string>
                {
                    ChatRoomId = room.Id,
                    Id = x.Id,
                    SendDate = x.SendDate,
                    SenderId = x.SenderUserId,
                    Sender = x.SenderUser,
                    Text = x.Text
                }),
                Participants = room.UserRooms.Select(x => x.User)
            };

            await notificator.Notificate(new ChatJoinNoitification<User, string>(new NewChatUser<User, string>
            {
                Room = roomModel,
                User = user
            }, room.UserRooms.Select(x => x.UserId).Where(x => x != userId).ToArray()));

            await notificator.Notificate(new NewChatNotification<User, string>(roomModel, new[] { userId }));

            return roomModel;
        }

        public async Task<ChatRoom<User, string>> GetRoomAsync(string roomId)
        {
            if (String.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId));
            }

            var item = await this.RoomsQuery.FirstOrDefaultAsync(x => x.Id == roomId);


            return item != null ? new ChatRoom<User, string>
            {
                Id = item.Id,
                IsPrivate = item.IsPrivate,
                Owner = item.OwnerUser,
                RoomName = item.RoomName,
                OwnerId = item.OwnerUserId,
                Messages = item.Messages.Select(x => new Message<User, string>
                {
                    ChatRoomId = x.ChatRoomId,
                    Id = x.Id,
                    SendDate = x.SendDate,
                    SenderId = x.SenderUserId,
                    Text = x.Text,
                    Sender = x.SenderUser
                }),
                Participants = item.UserRooms.Select(x => x.User)
            } : null;
        }

        public Task SetReadByUserAsync(string userId, string roomId, int? tillMessageId = null)
        {
            return messageRepo.SetReadByUserAsync(userId, roomId, tillMessageId);
        }

        public async Task<List<User>> GetPrivateRoomSupplementFor(string userId)
        {
            return (await this.userRepo.GetPrivateRoomSuplementAsync(userId)).Items.ToList();
        }
    }
}
