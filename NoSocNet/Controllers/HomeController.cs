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

namespace NoSocNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<UserEntity> userManager;
        public HomeController(ApplicationDbContext context, UserManager<UserEntity> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
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
