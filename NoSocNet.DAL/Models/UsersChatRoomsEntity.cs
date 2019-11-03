using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.DAL.Models
{
    public class UsersChatRoomsEntity
    {
        public string UserId { get; set; }

        public string ChatRoomId { get; set; }

        public ChatRoomEntity ChatRoom { get; set; }

        public User User { get; set; }
    }
}
