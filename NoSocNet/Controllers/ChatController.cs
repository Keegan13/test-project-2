using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using NoSocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService<User, string> chatService;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IApplicationUserStore<User> userStore;
        private readonly IIdentityService<User> identity;

        public ChatController(
            IChatService<User, string> hub,
            IIdentityService<User> identity,
            UserManager<User> userManager,
            IApplicationUserStore<User> userStore,
            IMapper mapper
            )
        {

            this.mapper = mapper;
            this.userStore = userStore;
            this.userManager = userManager;
            this.identity = identity;
            this.chatService = hub;
        }


        public async Task<IActionResult> Index()
        {
            var model = (await this.userStore.GetUsersAsync())
                .Where(x => x.Id != identity.CurrentUserId)
                .Select(x => mapper.Map<UserViewModel>(x));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rooms = await chatService.GetUserRooms(identity.CurrentUserId);

            return View(rooms);
        }

        public async Task<IActionResult> Invite(string roomId)
        {
            var users = await this.userStore.GetUsersAsync();
            var chatRoom = await this.chatService.GetRoomAsync(roomId);

            if (users != null && chatRoom != null)
            {

                return View(new InviteViewModel
                {
                    RoomId = roomId,
                    Users = users
                    .Where(x => chatRoom.Participants.Select(ps => ps.Id).Contains(x.Id))
                    .Select(x => mapper.Map<UserViewModel>(x))
                });
            }

            return StatusCode(404);
        }


        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var chats = await chatService.GetChatMessages(id);

            return View(chats);
        }

        //public async Task<IActionResult> Index(UserFilterModel filter)
        //{
        //    var users = await userManager.Users.Skip(filter.PageSize * filter.Page).Take(filter.PageSize).ToListAsync();

        //    return View(users);
        //}
        [HttpPost]
        public async Task<IActionResult> Sent(string text, string roomId)
        {

            var message = await this.chatService.Push(this.identity.CurrentUserId, roomId, text);


            return RedirectToAction("Room", new { roomId });
        }


        public async Task<IActionResult> Private(string Id)
        {
            var chatRoom = await this.chatService.JoinPrivateAsync(Id);

            var model = new ChatRoomViewModel
            {
                //ToDo: move to AutoMapper
                Id = chatRoom.Id,
                Messages = chatRoom.Messages.Select(x => mapper.Map<MessageViewModel>(x)).ToList(),
                Participants = chatRoom.Participants.Select(x => mapper.Map<UserViewModel>(x)).ToList(),
                IsPrivate = chatRoom.IsPrivate
            };

            return View("Room", model);
        }
        public async Task<IActionResult> Join(string userid, string roomId)
        {
            var chatRoom = await this.chatService.InviteToRoom(userid, roomId);




            return RedirectToAction("Room", new { id = chatRoom });
        }


        public async Task<IActionResult> Room(string roomId)
        {
            var room = await this.chatService.GetRoomAsync(roomId);

            if (room != null)
            {
                var model = new ChatRoomViewModel
                {
                    Id = room.Id,
                    Messages = room.Messages.Select(x => mapper.Map<MessageViewModel>(x)).ToList(),
                    Participants = room.Participants.Select(x => mapper.Map<UserViewModel>(x)).ToList(),
                    IsPrivate = room.IsPrivate,
                    Owner = mapper.Map<UserViewModel>(room.Owner),
                    OwnerId = room.OwnerId
                };

                return View(model);
            }

            return StatusCode(404);
        }
    }
}
