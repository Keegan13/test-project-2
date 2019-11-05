using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoSocNet.Infrastructure.Services;
using NoSocNet.Domain.Models;
using NoSocNet.Infrastructure.Seeding;
using NoSocNet.Models;
using NoSocNet.Infrastructure.Data;
using NoSocNet.Core.Interfaces;
using AutoMapper;
using NoSocNet.Extensions;

namespace NoSocNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<UserEntity> userManager;
        private readonly IIdentityService identity;
        private readonly IChatRoomRepository roomRepo;
        private readonly IMapper mapper;
        public HomeController(
            ApplicationDbContext context,
            UserManager<UserEntity> userManager,
            IIdentityService identity,
            IChatRoomRepository roomRepo,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
            this.roomRepo = roomRepo;
            this.identity = identity;
        }
        public async Task<IActionResult> Index(string roomId = null, string[] loadedRoomsIds = null)
        {
            string userId = this.identity.CurrentUserId;

            var rooms = (await roomRepo.GetRecentChatRoomsAsync(userId, loadedRoomsIds.Select(x => x).ToArray()));

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

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> FixDb()
        {

            var chatrooms = await context.ChatRooms.Where(x => x.UserRooms.Count() <= 1).ToListAsync();
            var roomsId = chatrooms.Select(x => x.Id).ToList();
            var messages = await context.Messages.Where(x => roomsId.Contains(x.ChatRoomId)).ToListAsync();
            context.RemoveRange(messages);
            context.RemoveRange(chatrooms);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public async Task<IActionResult> AddUsers(int id)
        //{
        //    var factory = new UserFactory(this.userManager, this.context);
        //    var newUsers = await factory.AddUsers(id);

        //    return Json(newUsers, new Newtonsoft.Json.JsonSerializerSettings
        //    {
        //        Formatting = Newtonsoft.Json.Formatting.Indented
        //    });
        //}
    }
}
