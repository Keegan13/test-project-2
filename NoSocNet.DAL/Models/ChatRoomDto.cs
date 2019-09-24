using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.DAL.Models
{
    public class ChatRoomDto
    {
        public ChatRoomDto()
        {
            this.UserRooms = new HashSet<UsersChatRoomsDto>();
            this.Messages = new HashSet<MessageDto>();
        }

        public string Id { get; set; }

        public bool IsPrivate { get; set; }

        public User Owner { get; set; }
        public string OwnerId { get; set; }
        public virtual ICollection<MessageDto> Messages { get; set; }
        public virtual ICollection<UsersChatRoomsDto> UserRooms { get; set; }
    }
}
