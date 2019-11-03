using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Core.Models
{
    public class ChatRoomDto
    {
        public string Id { get; set; }

        public bool IsPrivate { get; set; }

        public string RoomName { get; set; }

        public IEnumerable<UserDto> Participants { get; set; }

        public string OwnerId { get; set; }
        public UserDto Owner { get; set; }

        public bool HasUnread { get; set; }

        public IEnumerable<MessageDto> Messages { get; set; }

    }
}
