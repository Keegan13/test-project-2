using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.BLL.Models;
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
        private readonly IMapper mapper;
        public UsersController(
            IMapper mapper,
            IIdentityService<User> identity,
            IUserRepository<User, string> repo
            )
        {
            this.mapper = mapper;
            this.repo = repo;
            this.identity = identity;
        }

        //[HttpGet]

        //public async Task<UserViewModel> Get([FromRoute] string id)
        //{
        //    if (await repo.GetByIdAsync(id) is User user)
        //    {
        //        return mapper.Map<UserViewModel>(user);
        //    }

        //    return null;
        //}

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get(UserFilterModel filter)
        {
            string currentUserId = identity.CurrentUserId;

            var result = await repo.GetPrivateRoomSuplementAsync(currentUserId,
                new FilterBase { },
                new Paginator
                {
                    TailId = filter.Tailid
                });

            return result.Items.Select(x => mapper.Map<UserViewModel>(x)).ToArray();
        }
    }
}
