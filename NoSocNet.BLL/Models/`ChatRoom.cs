using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.BLL.Models
{
    public class ChatRoom<TUser, TKey>
    {
        public string Id { get; set; }

        public bool IsPrivate { get; set; }

        public string RoomName { get; set; }

        public IEnumerable<TUser> Participants { get; set; }

        public string OwnerId { get; set; }
        public TUser Owner { get; set; }

        public IEnumerable<Message<TUser, TKey>> Messages { get; set; }
    }
}
