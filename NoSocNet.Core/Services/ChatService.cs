using System;
using System.Collections.Generic;
using NoSocNet.Domain.Models;
using NoSocNet.Core.Interfaces;
using System.Threading.Tasks;
using NoSocNet.Core.Models;
using AutoMapper;
using NoSocNet.Core.Interfaces;
using System.Linq;

namespace NoSocNet.Core.Services
{
    public class ChatService : IChatService
    {
        private readonly IIdentityService identityService;
        private readonly INotificator notificator;
        private readonly IChatRoomRepository roomRepo;
        private readonly IUserRepository userRepo;
        private readonly IMessageRepository messageRepo;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ChatService(
            IUnitOfWork unitOfWork,
            IIdentityService identity,
            INotificator notificator,
            IChatRoomRepository roomRepo,
            IUserRepository userRepo,
            IMessageRepository messageRepo,
            IMapper mapper
            )
        {
            this.mapper = mapper;
            this.messageRepo = messageRepo;
            this.userRepo = userRepo;
            this.roomRepo = roomRepo;
            this.notificator = notificator;
            this.identityService = identity;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ChatRoomDto> GetOrCreatePrivateRoomWith(string userId)
        {
            string currentUserId = identityService.CurrentUserId;

            if (currentUserId == userId)
            {
                throw new ArgumentException("Same user error");
            }

            if (!(await this.userRepo.FindByIdAsync(userId) is UserEntity user))
            {
                throw new ArgumentException("User does not exists");
            }

            var privateRoom = await roomRepo.GetPrivateRoomAsync(currentUserId, userId);

            if (privateRoom == null)
            {
                ChatRoomEntity newChatRoom = new ChatRoomEntity
                {
                    IsPrivate = true,
                    OwnerUserId = currentUserId.ToLower(),
                    UserRooms = new List<UsersChatRoomsEntity> { new UsersChatRoomsEntity { UserId = currentUserId }, new UsersChatRoomsEntity { UserId = userId } }
                };
                string id = await roomRepo.CreateAsync(newChatRoom);
                privateRoom = await roomRepo.FindByIdAsync(id);
            }

            ChatRoomDto model = mapper.Map<ChatRoomEntity, ChatRoomDto>(privateRoom);

            await notificator.Notificate(new NewChatNotification(model, model.Participants.Select(x => x.Id)));

            return model;
        }

        public async Task<MessageDto> AddMessage(string userId, string roomId, string text)
        {
            if (!(await roomRepo.FindByIdAsync(roomId) is ChatRoomEntity chatRoom))
            {
                return null;
            }

            if (!(await userRepo.FindByIdAsync(userId) is UserEntity user))
            {
                return null;
            }

            var newMessage = new MessageEntity
            {
                ChatRoomId = chatRoom.Id,
                SenderUserId = user.Id,
                SendDate = DateTime.Now,
                Text = text
            };

            int id = await messageRepo.CreateAsync(newMessage);
            MessageEntity message = await messageRepo.FindByIdAsync(id);

            MessageDto model = mapper.Map<MessageEntity, MessageDto>(message);


            await this.notificator.Notificate(new MessageNotification(model, model.ChatRoom.Participants.Select(x => x.Id)));

            return model;
        }

        public async Task<ChatRoomDto> InviteToRoomAsync(string userId, string roomId)
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

            UserEntity user = await this.userRepo.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception($"Cannot find user with Id {userId}");
            }

            //var room = await this.roomRepo.FindByIdAsync(roomId);

            //if (room == null)
            //{
            //    throw new Exception($"Cannot find room with Id {roomId}");
            //}

            await roomRepo.AddParticipantsAsync(roomId, new[] { userId });

            var room = await this.roomRepo.FindByIdAsync(roomId);

            ChatRoomDto model = mapper.Map<ChatRoomEntity, ChatRoomDto>(room);

            await notificator.Notificate(new ChatJoinNoitification(new NewChatUser
            {
                Room = model,
                User = mapper.Map<UserEntity, UserDto>(user)
            }, model.Participants.Select(x => x.Id).Where(x => String.Compare(x, user.Id, true) != 0).ToArray()));

            await notificator.Notificate(new NewChatNotification(model, new[] { user.Id }));

            return model;
        }

        public Task SetReadByUserAsync(string userId, string roomId, int? tillMessageId = null)
        {
            return messageRepo.SetReadByUserAsync(userId, roomId, tillMessageId);
        }


    }
}
