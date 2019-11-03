using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Interfaces;
using NoSocNet.Domain.Interfaces;
using NoSocNet.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoSocNet.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository repo;
        private readonly IIdentityService identity;
        private readonly IMapper mapper;
        public UsersController(
            IMapper mapper,
            IIdentityService identity,
            IUserRepository repo
            )
        {
            this.mapper = mapper;
            this.repo = repo;
            this.identity = identity;
        }

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get(UserFilterModel filter)
        {
            string currentUserId = identity.CurrentUserId;

            var result = await repo.GetNonParticipantsForRoomAsync(currentUserId, filter.Keywords, 10, 0);

            return result.Select(x => mapper.Map<UserViewModel>(x)).ToArray();
        }
    }
}
