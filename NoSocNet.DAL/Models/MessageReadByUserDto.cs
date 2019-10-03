using System;

namespace NoSocNet.DAL.Models
{
    public class MessageReadByUserDto
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public int MessageId { get; set; }

        public MessageDto Message { get; set; }

        public DateTime? DateRead { get; set; }
    }
}
