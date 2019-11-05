using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Models;
using NoSocNet.Core.Interfaces;
using NoSocNet.Domain.Models;
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

        public class SeenInput
        {
            public string ChatRoomId { get; set; }
            public int? HeadMessageId { get; set; }
        }

        [HttpPut]
        public async Task<IActionResult> Seen([FromBody] SeenInput input)
        {
            string currentUserId = identity.CurrentUserId;
            await chatService.SetReadByUserAsync(currentUserId, input.ChatRoomId, input.HeadMessageId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string roomId = null, string[] loadedRoomsIds = null)
        {
            string userId = this.identity.CurrentUserId;

            var rooms = (await roomRepo.GetRecentChatRoomsAsync(userId, loadedRoomsIds.Select(x=>x).ToArray()));

            var reads = await roomRepo.GetHasUnreadAsync(rooms.Select(x => x.Id).ToArray(), userId);

            if (!String.IsNullOrEmpty(roomId) && rooms.Any(x => x.Id == roomId))
            {
                ViewBag.SelectedId = roomId.ToLower();
            }

            return View(new IndexViewModel
            {
                Rooms = rooms.Select(x =>
                {
                    var vm = mapper.Map<ChatRoomEntity, ChatRoomViewModel>(x);
                    vm.HasUnread = reads[vm.Id];
                    vm.RoomName = vm.GetRoomName(userId);
                    return vm;
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Invite(string chatRoomId)
        {
            string currentUserId = identity.CurrentUserId;
            var users = await this.userRepo.GetNonParticipantsForRoomAsync(currentUserId, null, 10);

            ChatRoomEntity chatRoom = await this.roomRepo.FindByIdAsync(chatRoomId);

            if (users != null && chatRoom != null)
            {
                var roomVm = mapper.Map<ChatRoomEntity, ChatRoomViewModel>(chatRoom);
                var model = new InviteUsersViewModel
                {
                    RoomName = roomVm.GetRoomName(identity.CurrentUserId),
                    RoomId = chatRoomId,
                    Users = users.Select(x => mapper.Map<UserEntity, UserViewModel>(x)).ToList()
                };

                return PartialView("_InviteUsers", model);
            }

            return StatusCode(404);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(string roomId, string[] people = null)
        {
            if (people != null && people.Count() > 0)
            {
                foreach (var userId in people)
                {
                    await this.chatService.InviteToRoomAsync(userId, roomId);
                }

            }

            var room = await this.roomRepo.FindByIdAsync(roomId);
            var model = mapper.Map<ChatRoomEntity, ChatRoomViewModel>(room);
            return PartialView("_chat", model);
        }


        [HttpPost]
        public async Task<IActionResult> InviteList(string keywords, int page = 1)
        {
            var result = await userRepo.GetNonParticipantsForRoomAsync(identity.CurrentUserId, keywords, 10, (page - 1) * 10);

            var model = result.Select(x => mapper.Map<UserEntity, UserViewModel>(x)).ToList();

            return PartialView("_userSelectList", model);
        }

        [HttpGet]
        public async Task<IActionResult> Private(string id)
        {
            try
            {
                var chatRoom = await this.chatService.GetOrCreatePrivateRoomWith(id);
                var currUserId = identity.CurrentUserId;

                var model = mapper.Map<ChatRoomDto, ChatRoomViewModel>(chatRoom);

                return Json(model);
                //return RedirectToAction("Index", new { roomId = chatRoom.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //ToDo log error
                //logger.Error("{0}",ex.ToString());
                return StatusCode(404);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Room([FromForm] string roomId)
        {
            var chatRoom = await roomRepo.FindByIdAsync(roomId);

            if (chatRoom == null)
            {
                return BadRequest("Room not found");
            }

            string currUserId = identity.CurrentUserId;

            ChatRoomViewModel result = mapper.Map<ChatRoomEntity, ChatRoomViewModel>(chatRoom);

            return Json(result);
        }
        public IActionResult Search(string q)
        {
            return Json(new[] { "1", "123" });
        }

        [HttpPost]
        public async Task<IActionResult> Rooms([FromForm] string keywords = null)
        {
            var currUserId = identity.CurrentUserId;
            return Json(new[] { "1", "123" });
            var rooms = await roomRepo.SearchRoomsAsync(currUserId, keywords);
            var result = rooms.Select(x => mapper.Map<ChatRoomEntity, ChatRoomViewModel>(x)).ToList();

            return Json(result);
        }
    }
}
