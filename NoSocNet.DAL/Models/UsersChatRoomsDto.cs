using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.DAL.Models
{
    public class UsersChatRoomsDto
    {
        public string UserId { get; set; }
        public string ChatRoomId { get; set; }

        public ChatRoomDto ChatRoom { get; set; }
        public User User { get; set; }
    }
}
