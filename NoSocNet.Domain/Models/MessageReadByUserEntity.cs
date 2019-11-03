using System;

namespace NoSocNet.Domain.Models
{
    public class MessageReadByUserEntity
    {
        public string UserId { get; set; }

        public UserEntity User { get; set; }

        public int MessageId { get; set; }

        public MessageEntity Message { get; set; }

        public DateTime? DateRead { get; set; }
    }
}
