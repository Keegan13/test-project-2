using Microsoft.AspNetCore.Identity;
using NoSocNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Domain.Models
{
    public class UserEntity : IdentityUser, IIdentifiable<string>
    {
        public ICollection<MessageEntity> Messages { get; set; }
        public ICollection<UsersChatRoomsEntity> UserRooms { get; set; }
    }
}
