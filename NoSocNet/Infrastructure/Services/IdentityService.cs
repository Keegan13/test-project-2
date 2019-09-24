using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{
    public class IdentityService : IIdentityService<User>
    {
        private readonly HttpContext Context;
        private readonly UserManager<User> userManager;
        public IdentityService(
            IHttpContextAccessor httpAccessor,
            UserManager<User> userManager
            )
        {
            this.Context = httpAccessor.HttpContext;
            this.userManager = userManager;
        }
        public string CurrentUserId => CurrentUser.GetAwaiter().GetResult().Id;

        public Task<User> CurrentUser => userManager.GetUserAsync(Context.User as ClaimsPrincipal);
    }
}
