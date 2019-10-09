using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.DAL.Models
{
    public class ChatRoomEntity
    {
        public ChatRoomEntity()
        {
            this.UserRooms = new HashSet<UsersChatRoomsEntity>();
            this.Messages = new HashSet<MessageEntity>();
        }

        public string Id { get; set; }

        public string RoomName { get; set; }

        public bool IsPrivate { get; set; }

        public User OwnerUser { get; set; }
        public string OwnerUserId { get; set; }
        public virtual ICollection<MessageEntity> Messages { get; set; }
        public virtual ICollection<UsersChatRoomsEntity> UserRooms { get; set; }
    }
}
