using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NoSocNet.Core.Interfaces;
using NoSocNet.Domain.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace NoSocNet.Infrastructure.Services.Core
{
    public class HttpContextIdentityService : IIdentityService
    {
        private readonly HttpContext Context;
        private readonly UserManager<UserEntity> userManager;
        public HttpContextIdentityService(
            IHttpContextAccessor httpAccessor,
            UserManager<UserEntity> userManager
            )
        {
            this.Context = httpAccessor.HttpContext;
            this.userManager = userManager;
        }
        public string CurrentUserId => CurrentUser.GetAwaiter().GetResult().Id;

        public Task<UserEntity> CurrentUser => userManager.GetUserAsync(Context.User as ClaimsPrincipal);

        Task<NoSocNet.Domain.Models.UserEntity> IIdentityService.CurrentUser => throw new NotImplementedException();
    }
}
