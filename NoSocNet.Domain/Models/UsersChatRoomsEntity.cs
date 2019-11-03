using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Domain.Models
{
    public partial class UsersChatRoomsEntity
    {
        public string UserId { get; set; }

        public string ChatRoomId { get; set; }

        public ChatRoomEntity ChatRoom { get; set; }

        public UserEntity User { get; set; }
    }
}
