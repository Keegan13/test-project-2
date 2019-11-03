using NoSocNet.Domain.Models;
using System.Threading.Tasks;

namespace NoSocNet.Core.Interfaces
{
    public interface IIdentityService
    {
        string CurrentUserId { get; }
        Task<UserEntity> CurrentUser { get; }
    }
}
