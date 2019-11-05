using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Interfaces;
using NoSocNet.Domain.Models;
using NoSocNet.Extensions;
using NoSocNet.Models;

namespace NoSocNet.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IMessageRepository messagesRepo;
        private readonly IUserRepository usersRepo;
        private readonly IChatRoomRepository chatRoomsRepo;
        private readonly IIdentityService identity;
        private readonly IMapper mapper;

        public SearchController(
            IMessageRepository messagesRepo,
            IUserRepository usersRepo,
            IChatRoomRepository chatRoomsRepo,
            IIdentityService identity,
            IMapper mapper
            )
        {
            this.messagesRepo = messagesRepo;
            this.identity = identity;
            this.chatRoomsRepo = chatRoomsRepo;
            this.usersRepo = usersRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<SearchResult> Get(string keywords)
        {
            string currentUserId = identity.CurrentUserId;
            var users = await usersRepo.Search(keywords, currentUserId);
            var messages = await messagesRepo.Search(keywords, currentUserId);
            var chatRooms = await chatRoomsRepo.Search(keywords, currentUserId);

            var result = new SearchResult
            {
                Users = users.Select(x => mapper.Map<UserEntity, UserViewModel>(x)).ToArray(),
                Messages = messages.Select(x => mapper.Map<MessageEntity, MessageViewModel>(x)).ToArray(),
                ChatRooms = chatRooms.Select(x => mapper.Map<ChatRoomEntity, ChatRoomViewModel>(x)).Select(x =>
                {
                    x.RoomName = x.GetRoomName(currentUserId);
                    x.Participants = String.IsNullOrWhiteSpace(keywords) ? x.Participants : x.Participants.Where(u => u.UserName.Contains(keywords) || u.Email.Contains(keywords)).ToList();
                    return x;
                }).ToArray()
            };

            return result;
        }


        [HttpGet, Route("messages")]
        public async Task<MessageViewModel[]> Messages(string keywords, int chunk = 1)
        {
            string currentUserId = identity.CurrentUserId;
            var messages = await messagesRepo.Search(keywords, currentUserId, (chunk - 1) * 10, 10);

            return messages.Select(x => mapper.Map<MessageEntity, MessageViewModel>(x)).ToArray();
        }

        [HttpGet, Route("users")]
        public async Task<UserViewModel[]> Users(string keywords, int chunk = 1)
        {
            string currentUserId = identity.CurrentUserId;
            var users = await usersRepo.Search(keywords, currentUserId, (chunk - 1) * 10, 10);

            return users.Select(x => mapper.Map<UserEntity, UserViewModel>(x)).ToArray();
        }
        [HttpGet, Route("chat-rooms")]
        public async Task<ChatRoomViewModel[]> ChatRooms(string keywords, int chunk = 1)
        {
            string currentUserId = identity.CurrentUserId;
            var chatRooms = await chatRoomsRepo.Search(keywords, currentUserId, (chunk - 1) * 10, 10);
            return chatRooms.Select(x => mapper.Map<ChatRoomEntity, ChatRoomViewModel>(x)).ToArray();
        }
    }
}
