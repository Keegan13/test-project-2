using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoSocNet.BLL.Models;
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
            IChatService<User, string> chatService,
            IIdentityService<User> identity,
            UserManager<User> userManager,
            IApplicationUserStore<User> userStore,
            IMapper mapper
            //ILogger logger
            )
        {

            this.mapper = mapper;
            this.userStore = userStore;
            this.userManager = userManager;
            this.identity = identity;
            this.chatService = chatService;
        }


        public async Task<IActionResult> Users()
        {
            var model = (await this.userStore.GetUserListAsync())
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

        [NonAction]
        public static string GetRoomName(ChatRoom<User, string> room, string currentUserId)
        {
            if (!String.IsNullOrEmpty(room.RoomName))
            {
                return room.RoomName;
            }

            if (room.IsPrivate)
            {
                return room.Participants.FirstOrDefault(x => x.Id != currentUserId)?.UserName;
            }

            return room.Participants.Where(x => x.Id != currentUserId).Aggregate("# ", (agg, next) => agg += next.UserName + ", ").Trim(' ', ',');
        }

        public async Task<IActionResult> Index()
        {

            var rooms = await chatService.GetUserRooms(identity.CurrentUserId);

            string userId = this.identity.CurrentUserId;
            var model = rooms.Select(x => new ChatRoomViewModel
            {
                Id = x.Id,
                RoomName = GetRoomName(x, userId),
                OwnerId = x.OwnerId,
                IsPrivate = x.IsPrivate,
                Messages = x.Messages.Select(ms => mapper.Map<MessageViewModel>(ms)).ToList(),
                Owner = mapper.Map<UserViewModel>(x.Owner),
                Participants = x.Participants.Select(u => mapper.Map<UserViewModel>(u)).ToList()
            });

            return View(model);
        }

        public async Task<IActionResult> Invite(string id)
        {
            IList<User> users = await this.userStore.GetUserListAsync();
            ChatRoom<User, string> chatRoom = await this.chatService.GetRoomAsync(id);

            if (users != null && chatRoom != null)
            {
                var model = new InviteViewModel
                {
                    RoomId = id,
                    Users = users
                        .Where(x => !chatRoom.Participants.Select(ps => ps.Id).Contains(x.Id))
                        .Select(x => mapper.Map<UserViewModel>(x))
                };

                return PartialView("_InviteUsers", model);
            }

            return StatusCode(404);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(string roomId, string[] people = null)
        {
            if (people != null && people.Count() > 0)
            {
                foreach (var userId in people)
                {
                    await this.chatService.InviteToRoom(userId, roomId);
                }

            }

            var room = await this.chatService.GetRoomAsync(roomId);
            var model = new ChatRoomViewModel
            {
                Id = room.Id,
                Messages = room.Messages.Select(x => mapper.Map<MessageViewModel>(x)).ToList(),
                Participants = room.Participants.Select(x => mapper.Map<UserViewModel>(x)).ToList(),
                IsPrivate = room.IsPrivate,
                Owner = mapper.Map<UserViewModel>(room.Owner),
                RoomName = GetRoomName(room, identity.CurrentUserId),
                OwnerId = room.OwnerId
            };

            return PartialView("_chatTab", model);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var chats = await chatService.GetChatMessages(id);

            return View(chats);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sent(string text, string roomId)
        {
            if (await this.chatService.Push(this.identity.CurrentUserId, roomId, text) is Message<User, string> message)
            {
                return new JsonResult(new MessageViewModel
                {
                    ChatRoomId = message.ChatRoomId,
                    Id = message.Id,
                    SendDate = message.SendDate,
                    SenderId = message.SenderId,
                    Text = message.Text,
                    SenderUserName = message.Sender.UserName
                });
            }

            return NotFound(new { roomId });
        }


        public async Task<IActionResult> Private(string Id)
        {


            try
            {
                var chatRoom = await this.chatService.JoinPrivateAsync(Id);

                var model = new ChatRoomViewModel
                {
                    //ToDo: move to AutoMapper
                    Id = chatRoom.Id,
                    Messages = chatRoom.Messages.Select(x => mapper.Map<MessageViewModel>(x)).ToList(),
                    Participants = chatRoom.Participants.Select(x => mapper.Map<UserViewModel>(x)).ToList(),
                    IsPrivate = chatRoom.IsPrivate,
                    OwnerId = identity.CurrentUserId,
                    RoomName = chatRoom.Participants.FirstOrDefault(x => x.Id != identity.CurrentUserId)?.UserName,
                    Owner = mapper.Map<UserViewModel>(chatRoom.Owner)
                };

                return View("Room", model);
            }
            catch (Exception ex)
            {
                //ToDo log error
                //logger.Error("{0}",ex.ToString());
                return StatusCode(404);
            }
        }
        public async Task<IActionResult> Join(string userid, string roomId)
        {
            try
            {
                var chatRoom = await this.chatService.InviteToRoom(userid, roomId);


                return RedirectToAction("Room", new { id = chatRoom });
            }
            catch (Exception ex)
            {
                //log error
                throw ex;
            }
        }


        public async Task<IActionResult> Room(string roomId, bool isFf = false)
        {
            if (isFf)
            {
                ViewBag.isForeverFrame = true;
            }
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
                    RoomName = GetRoomName(room, identity.CurrentUserId),
                    OwnerId = room.OwnerId
                };

                return View(model);
            }

            return StatusCode(404);
        }
    }
}
