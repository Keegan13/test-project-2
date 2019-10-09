using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.BLL.Models;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using NoSocNet.Extensions;
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
        private readonly IIdentityService<User> identity;
        private readonly IChatRoomRepository<User, string> roomRepo;
        private readonly IMessageRepository<User, string> messageRepo;
        private readonly IUserRepository<User, string> userRepo;

        public ChatController(
            IUserRepository<User, string> userRepo,
            IMessageRepository<User, string> messageRepo,
            IChatService<User, string> chatService,
            IIdentityService<User> identity,
            UserManager<User> userManager,
            IMapper mapper,
            IChatRoomRepository<User, string> roomRepo
            //ILogger logger
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
        public async Task<IActionResult> Seen(string roomId, int? id = null)
        {
            string currUserId = identity.CurrentUserId;
            await chatService.SetReadByUserAsync(currUserId, roomId, id);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Index(string roomId = null)
        {
            string userId = this.identity.CurrentUserId;

            var rooms = (await roomRepo.GetRoomsAsync(userId)).Items
                .Select(x => new ChatRoomViewModel
                {
                    Id = x.Id,
                    RoomName = x.GetRoomName(userId),
                    OwnerId = x.OwnerId,
                    IsPrivate = x.IsPrivate,
                    Messages = x.Messages.Select(ms => mapper.Map<MessageViewModel>(ms)).ToList(),
                    Owner = mapper.Map<UserViewModel>(x.Owner),
                    Participants = x.Participants.Select(u => mapper.Map<UserViewModel>(u)).ToList(),
                    HasUnread = x.HasUnread
                });

            var users = (await userRepo.GetPrivateRoomSuplementAsync(userId))
                .Items
                .Select(x => new UserViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName
                })
                .OrderBy(x => x.UserName);

            if (!String.IsNullOrEmpty(roomId) && rooms.Any(x => x.Id == roomId))
            {
                ViewBag.SelectedId = roomId;
            }

            return View(new IndexViewModel
            {
                Rooms = rooms,
                Users = users
            });
        }

        public async Task<IActionResult> Invite(string id)
        {
            var currUserId = identity.CurrentUserId;
            IList<User> users = (await this.userRepo.GetRoomUserSuplementAsync(currUserId, id)).Items.ToList();

            ChatRoom<User, string> chatRoom = await this.roomRepo.GetRoom(id);

            if (users != null && chatRoom != null)
            {
                var model = new InviteViewModel
                {
                    RoomName = chatRoom.GetRoomName(identity.CurrentUserId),
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
                    await this.chatService.InviteToRoomAsync(userId, roomId);
                }

            }

            var room = await this.roomRepo.GetRoom(roomId);
            var model = new ChatRoomViewModel
            {
                Id = room.Id,
                Messages = room.Messages.Select(x => mapper.Map<MessageViewModel>(x)).ToList(),
                Participants = room.Participants.Select(x => mapper.Map<UserViewModel>(x)).ToList(),
                IsPrivate = room.IsPrivate,
                Owner = mapper.Map<UserViewModel>(room.Owner),
                RoomName = room.GetRoomName(identity.CurrentUserId),
                OwnerId = room.OwnerId
            };

            return PartialView("_chat", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Sent(string text, string roomId)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Ok();
            }

            if (await this.chatService.AddMessage(this.identity.CurrentUserId, roomId, text) is Message<User, string> message)
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

        [HttpGet]
        public async Task<IActionResult> Private(string id)
        {
            try
            {
                var chatRoom = await this.chatService.GetOrCreatePrivateRoomWith(id);
                var currUserId = identity.CurrentUserId;

                var model = new ChatRoomViewModel
                {
                    //ToDo: move to AutoMapper
                    Id = chatRoom.Id,
                    Messages = chatRoom.Messages.Select(x => mapper.Map<MessageViewModel>(x)).ToList(),
                    Participants = chatRoom.Participants.Select(x => mapper.Map<UserViewModel>(x)).ToList(),
                    IsPrivate = chatRoom.IsPrivate,
                    OwnerId = identity.CurrentUserId,
                    RoomName = chatRoom.GetRoomName(currUserId),
                    Owner = mapper.Map<UserViewModel>(chatRoom.Owner)
                };

                return Json(model);
                //return RedirectToAction("Index", new { roomId = chatRoom.Id });
            }
            catch (Exception ex)
            {
                //ToDo log error
                //logger.Error("{0}",ex.ToString());
                return StatusCode(404);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Room([FromForm] string roomId)
        {
            var chatRoom = await roomRepo.GetRoom(roomId);

            if (chatRoom == null)
            {
                return BadRequest("Room not found");
            }

            string currUserId = identity.CurrentUserId;

            var result = new ChatRoomViewModel
            {
                Id = chatRoom.Id,
                IsPrivate = chatRoom.IsPrivate,
                RoomName = chatRoom.GetRoomName(currUserId),
                Owner = chatRoom.Owner == null ? null : new UserViewModel
                {
                    Email = chatRoom.Owner.Email,
                    Id = chatRoom.Owner.Id,
                    UserName = chatRoom.Owner.UserName
                },
                OwnerId = chatRoom.OwnerId,
                Participants = chatRoom.Participants.Select(x => new UserViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName
                }).ToList(),
                Messages = chatRoom.Messages.Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    Text = m.Text,
                    SendDate = m.SendDate,
                    SenderId = m.SenderId,
                    SenderUserName = m.Sender.UserName,
                    ChatRoomId = m.ChatRoomId,
                    ReadByUsersIds = m.ReadByUsers.Select(r => r.Id).ToList()
                }).ToList()
            };

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Rooms([FromForm] string tailId = null, [FromForm] string keywords = null)
        {
            var currUserId = identity.CurrentUserId;
            var rooms = await roomRepo.GetRoomsAsync(currUserId, new FilterBase { Keywords = keywords }, new Paginator
            {
                TailId = tailId
            });

            var result = rooms.Items.Select(x => new ChatRoomViewModel
            {
                Id = x.Id,
                IsPrivate = x.IsPrivate,
                RoomName = x.GetRoomName(currUserId),
                Owner = x.Owner == null ? null : new UserViewModel
                {
                    Email = x.Owner.Email,
                    Id = x.Owner.Id,
                    UserName = x.Owner.UserName
                },
                OwnerId = x.OwnerId,
                Messages = x.Messages.Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    Text = m.Text,
                    SendDate = m.SendDate,
                    SenderId = m.SenderId,
                    SenderUserName = m.Sender.UserName,
                    ChatRoomId = m.ChatRoomId,
                    ReadByUsersIds = m.ReadByUsers.Select(r => r.Id).ToList()

                }).ToList(),
                Participants = x.Participants.Select(u => new UserViewModel
                {
                    Email = u.Email,
                    Id = u.Id,
                    UserName = u.UserName
                }).ToList()
            });

            return Json(result);
        }
    }
}
