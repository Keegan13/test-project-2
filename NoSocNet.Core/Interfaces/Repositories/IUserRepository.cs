﻿using NoSocNet.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetNonParticipantsForRoomAsync(string chatRoomId, string userId, string keywords = null, int take = 10, int skip = 0);

        Task<UserEntity> FindByIdAsync(string id);

        Task<ICollection<UserEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10);
    }
}
