using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoSocNet.DAL.Context;
using NoSocNet.DAL.Models;
using NoSocNet.Infrastructure.Seeding;
using NoSocNet.Models;

namespace NoSocNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        public HomeController(ApplicationDbContext context, UserManager<User> userManager)
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

        private async Task<IEnumerable<string>> RandomUsers(int count)
        {
            return await context.Users.OrderBy(x => Guid.NewGuid()).Take(count).Select(x => x.Id).ToListAsync();
        }
        public async Task<IActionResult> Seed()
        {
            Random R = new Random();
            int userCount = await context.Users.CountAsync();

            var rooms = await context.ChatRooms.Where(x => x.UserRooms.Count() == 0)
                .Select(x => x.Id).ToListAsync();

            foreach (var room in rooms)
            {
                var users = await RandomUsers(R.Next(2, 5));

                await context.AddRangeAsync(users.Select(x => new UsersChatRoomsEntity
                {
                    ChatRoomId = room,
                    UserId = x
                }).ToArray());
                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();
            return Json(rooms);
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
