using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.DAL.Models
{
    public class MessageEntity
    {
        public MessageEntity()
        {
            this.ReadByUsers = new HashSet<MessageReadByUserEntity>();
        }
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime SendDate { get; set; }

        public string ChatRoomId { get; set; }

        public virtual ChatRoomEntity ChatRoom { get; set; }

        public string SenderUserId { get; set; }

        public virtual User SenderUser { get; set; }

        public ICollection<MessageReadByUserEntity> ReadByUsers { get; set; }
    }
}
