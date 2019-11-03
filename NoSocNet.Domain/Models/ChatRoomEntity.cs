using NoSocNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Domain.Models
{
    public class ChatRoomEntity : IIdentifiable<string>
    {
        public ChatRoomEntity()
        {
            this.UserRooms = new HashSet<UsersChatRoomsEntity>();
            this.Messages = new HashSet<MessageEntity>();
        }

        public string Id { get; set; }

        public string RoomName { get; set; }

        public bool IsPrivate { get; set; }

        public UserEntity OwnerUser { get; set; }
        public string OwnerUserId { get; set; }
        public virtual ICollection<MessageEntity> Messages { get; set; }
        public virtual ICollection<UsersChatRoomsEntity> UserRooms { get; set; }
    }
}
