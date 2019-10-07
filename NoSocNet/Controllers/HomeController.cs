﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
