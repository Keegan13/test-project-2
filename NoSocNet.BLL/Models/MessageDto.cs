using System;
using System.Collections.Generic;


namespace NoSocNet.Core.Models
{
    public class MessageDto
    {
        public MessageDto()
        {
            this.ReadByUsers = new List<UserDto>();
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime SendDate { get; set; }

        public string ChatRoomId { get; set; }

        public virtual ChatRoomDto ChatRoom { get; set; }

        public virtual UserDto Sender { get; set; }

        public IEnumerable<UserDto> ReadByUsers { get; set; }
    }
}
