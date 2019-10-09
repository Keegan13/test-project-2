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
    public class ChatService : IChatService<User, string>
    {
        private readonly IIdentityService<User> identityService;
        private readonly INotificator<string> notificator;
        private readonly IChatRoomRepository<User, string> roomRepo;
        private readonly IUserRepository<User, string> userRepo;
        private readonly IMessageRepository<User, string> messageRepo;
        private readonly IUnitOfWork unitOfWork;

        public ChatService(
            IUnitOfWork unitOfWork,
            IIdentityService<User> identity,
            INotificator<string> notificator,
            IChatRoomRepository<User, string> roomRepo,
            IUserRepository<User, string> userRepo,
            IMessageRepository<User, string> messageRepo
            )
        {
            this.messageRepo = messageRepo;
            this.userRepo = userRepo;
            this.roomRepo = roomRepo;
            this.notificator = notificator;
            this.identityService = identity;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ChatRoom<User, string>> GetOrCreatePrivateRoomWith(string userId)
        {
            string currentUserId = identityService.CurrentUserId;

            if (currentUserId == userId)
            {
                throw new ArgumentException("Same user error");
            }

            if (!await this.userRepo.Exists(userId))
            {
                throw new ArgumentException("User does not exists");
            }

            var privateRoom = await roomRepo.GetPrivateRoom(currentUserId, userId);

            if (privateRoom == null)
            {
                await roomRepo.CreatePrivateRoom(currentUserId, userId);
                await unitOfWork.SaveAsync();
                privateRoom = await roomRepo.GetPrivateRoom(currentUserId, userId);
            }

            await notificator.Notificate(new NewChatNotification<User, string>(privateRoom, privateRoom.Participants.Select(x => x.Id)));

            return privateRoom;
        }

        public async Task<Message<User, string>> AddMessage(string userId, string roomId, string text)
        {
            var newMessage = new Message<User, string>
            {
                ChatRoomId = roomId,
                SenderId = userId,
                Text = text
            };

            await messageRepo.AddMessage(newMessage);

            await this.notificator.Notificate(new MessageNotification<User, string>(newMessage, newMessage.ChatRoom.Participants.Select(x => x.Id)));

            return newMessage;
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

            User user = await this.userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception($"Cannot find user with Id {userId}");
            }

            var room = await this.roomRepo.GetRoom(roomId);

            if (room == null)
            {
                throw new Exception($"Cannot find room with Id {roomId}");
            }

            await roomRepo.AddUserToRoom(userId, roomId);
            await unitOfWork.SaveAsync();

            await notificator.Notificate(new ChatJoinNoitification<User, string>(new NewChatUser<User, string>
            {
                Room = room,
                User = user
            }, room.Participants.Select(x => x.Id)));

            await notificator.Notificate(new NewChatNotification<User, string>(room, new[] { userId }));

            return room;
        }

        public Task SetReadByUserAsync(string userId, string roomId, int? tillMessageId = null)
        {
            return messageRepo.SetReadByUserAsync(userId, roomId, tillMessageId);
        }

    }
}
