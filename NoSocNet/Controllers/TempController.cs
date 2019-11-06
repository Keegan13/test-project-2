using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Interfaces;
using NoSocNet.Domain.Models;
using NoSocNet.Extensions;
using NoSocNet.Models;

namespace NoSocNet.Controllers
{
    [Authorize]
    [Route("chat")]
    public class TempController : Controller
    {
        private readonly IChatService chatService;
        private readonly IMapper mapper;
        private readonly IIdentityService identity;
        private readonly IChatRoomRepository roomRepo;
        private readonly IUserRepository userRepo;

        public TempController(
            IUserRepository userRepo,
            IChatService chatService,
            IIdentityService identity,
            IMapper mapper,
            IChatRoomRepository roomRepo
            )
        {
            this.userRepo = userRepo;

            this.roomRepo = roomRepo;
            this.mapper = mapper;

            this.identity = identity;
            this.chatService = chatService;
        }

        [HttpGet]
        [Route("invite")]
        public async Task<IActionResult> Invite(string chatRoomId)
        {
            string currentUserId = identity.CurrentUserId;
            var users = await this.userRepo.GetNonParticipantsForRoomAsync(chatRoomId, currentUserId, null, 10);

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

                return PartialView("_inviteUsers", model);
            }

            return StatusCode(404);
        }

        [HttpPost]
        [Route("invite")]
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
            return PartialView("_chatTab", model);
        }


        [HttpPost]
        [Route("inviteList")]
        public async Task<IActionResult> InviteList(string chatRoomId, string keywords, int page = 1)
        {
            var result = await userRepo.GetNonParticipantsForRoomAsync(chatRoomId, identity.CurrentUserId, keywords, 10, (page - 1) * 10);

            var model = result.Select(x => mapper.Map<UserEntity, UserViewModel>(x)).ToList();

            return PartialView("_userSelectList", model);
        }
    }
}