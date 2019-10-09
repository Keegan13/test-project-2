using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using NoSocNet.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoSocNet.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository<User, string> repo;
        private readonly IIdentityService<User> identity;
        public UsersController(
            IIdentityService<User> identity,
            IUserRepository<User, string> repo
            )
        {
            this.repo = repo;
            this.identity = identity;
        }

        // GET: api/<controller>
        
        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get(string tailId)
        {
            var currUserId = identity.CurrentUserId;
            var pagedList = await repo.GetPrivateRoomSuplementAsync(currUserId, null, new Paginator
            {
                TailId = tailId,
                PageSize = 10
            });

            return pagedList.Items.Select(x => new UserViewModel
            {
                Email = x.Email,
                Id = x.Id,
                UserName = x.UserName
            });
        }
    }
}
