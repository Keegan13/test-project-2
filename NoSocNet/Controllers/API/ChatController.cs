using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Interfaces.Repositories;
using NoSocNet.Core.Models;
using NoSocNet.Domain.Models;
using NoSocNet.Extensions;
using NoSocNet.Models;
using NoSocNet.Models.API;

namespace NoSocNet.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;
        private readonly UserManager<UserEntity> userManager;
        private readonly IMapper mapper;
        private readonly IIdentityService identity;
        private readonly IChatRoomRepository roomRepo;
        private readonly IMessageRepository messageRepo;
        private readonly IUserRepository userRepo;

        public ChatController(
            IUserRepository userRepo,
            IMessageRepository messageRepo,
            IChatService chatService,
            IIdentityService identity,
            UserManager<UserEntity> userManager,
            IMapper mapper,
            IChatRoomRepository roomRepo
            )
        {
            this.userRepo = userRepo;
            this.messageRepo = messageRepo;
            this.roomRepo = roomRepo;
            this.mapper = mapper;
            this.userManager = userManager;
            this.identity = identity;
            this.chatService = chatService;
        }

        [HttpPut]
        [Route("seen")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Seen([FromBody] SeenInput input)
        {
            string currentUserId = identity.CurrentUserId;
            await chatService.SetReadByUserAsync(currentUserId, input.ChatRoomId, input.HeadMessageId);

            return Accepted(new { input });
        }

        [HttpGet]
        [Route("invite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserViewModel[]>> Invite([FromQuery] string chatRoomId, [FromQuery] string keywords = null, [FromQuery] int page = 1)
        {
            var result = await userRepo.GetNonParticipantsForRoomAsync(chatRoomId, identity.CurrentUserId, keywords, 10, (page - 1) * 10);
            UserViewModel[] users = result.Select(x => mapper.Map<UserEntity, UserViewModel>(x)).ToArray();

            return users;
        }

        [HttpPost]
        [Route("invite")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult<ChatRoomViewModel>> Invite([FromBody] InviteInput input)
        {
            ChatRoomDto room = await this.chatService.InviteToRoomAsync(input.UserId, input.ChatRoomId);

            if (room == null)
            {
                return BadRequest();
            }

            ChatRoomViewModel model = mapper.Map<ChatRoomDto, ChatRoomViewModel>(room);

            return model;
        }

        [HttpGet]
        [Route("private")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChatRoomViewModel>> Private([FromQuery] string chatRoomId)
        {
            try
            {
                ChatRoomDto chatRoom = await this.chatService.GetOrCreatePrivateRoomWith(chatRoomId);

                if (chatRoom == null)
                {
                    return NotFound();
                }

                String currUserId = identity.CurrentUserId;

                ChatRoomViewModel model = mapper.Map<ChatRoomDto, ChatRoomViewModel>(chatRoom);

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //ToDo log error
                //logger.Error("{0}",ex.ToString());
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("room")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ChatRoomViewModel>> Room([FromQuery] string chatRoomId)
        {
            var chatRoom = await roomRepo.FindByIdAsync(chatRoomId);

            if (chatRoom == null)
            {
                return BadRequest("Room not found");
            }

            ChatRoomViewModel result = mapper.Map<ChatRoomEntity, ChatRoomViewModel>(chatRoom);

            return result;
        }

        [HttpGet]
        [Route("message")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageViewModel>> Message([FromQuery] int id)
        {
            if (await this.messageRepo.FindByIdAsync(id) is MessageEntity message)
            {
                return mapper.Map<MessageEntity, MessageViewModel>(message);
            }

            return NotFound();
        }


        [HttpPost]
        [Route("message")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Sent([FromBody] MessageInput input)
        {
            if (await this.chatService.AddMessage(this.identity.CurrentUserId, input.ChatRoomId, input.Text) is MessageDto message)
            {
                return Created(Url.Action("Message", "Chat", new { id = message.Id }), mapper.Map<MessageDto, MessageViewModel>(message));
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("load-chats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChatRoomViewModel[]>> Chats([FromBody] RecentChatsInput input)
        {
            string currentUserId = identity.CurrentUserId;
            var chats = await roomRepo.GetRecentChatRoomsAsync(currentUserId, input.Loaded.ToArray(), 10);

            return chats.Select(x => mapper.Map<ChatRoomEntity, ChatRoomViewModel>(x)).ToArray();
        }

        [HttpGet]
        [Route("load-messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MessageViewModel[]>> Messages([FromQuery]string chatRoomid, [FromQuery] int? tailMessageId)
        {
            var messages = await messageRepo.GetMessagesAsync(chatRoomid, 10, tailMessageId);

            return messages.Select(x => mapper.Map<MessageEntity, MessageViewModel>(x)).ToArray();
        }
    }
}