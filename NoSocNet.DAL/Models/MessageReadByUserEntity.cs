using System;

namespace NoSocNet.DAL.Models
{
    public class MessageReadByUserEntity
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public int MessageId { get; set; }

        public MessageEntity Message { get; set; }

        public DateTime? DateRead { get; set; }
    }
}
