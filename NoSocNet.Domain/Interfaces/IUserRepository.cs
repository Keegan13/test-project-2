using NoSocNet.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetNonParticipantsForRoomAsync(string userId, string keywords = null, int take = 10, int skip = 0);

        Task<UserEntity> FindByIdAsync(string id);
    }
}
