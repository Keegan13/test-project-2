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
            this.UserRooms = new HashSet<UsersChatRoomsEntity>();
            this.Messages = new HashSet<MessageEntity>();
        }
        public virtual ICollection<UsersChatRoomsEntity> UserRooms { get; set; }

        public virtual ICollection<MessageEntity> Messages { get; set; }
    }
}
