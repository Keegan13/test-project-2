using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class ChatJoinViewModel
    {
        public ChatRoomViewModel ChatRoom { get; set; }

        public UserViewModel User { get; set; }
    }
}
