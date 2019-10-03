using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.DAL.Models
{
    public class MessageDto
    {
        public MessageDto()
        {
            this.ReadByUsers = new HashSet<MessageReadByUserDto>();
        }
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime SendDate { get; set; }

        public string ChatRoomId { get; set; }

        public virtual ChatRoomDto ChatRoom { get; set; }

        public string SenderId { get; set; }

        public virtual User Sender { get; set; }

        public ICollection<MessageReadByUserDto> ReadByUsers { get; set; }
    }
}
