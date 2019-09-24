using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.DAL.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.UserRooms = new HashSet<UsersChatRoomsDto>();
            this.Messages = new HashSet<MessageDto>();
        }
        public virtual ICollection<UsersChatRoomsDto> UserRooms { get; set; }

        public virtual ICollection<MessageDto> Messages { get; set; }
    }
}
