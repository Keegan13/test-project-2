using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.BLL.Models
{
    public class Message<TUser, TUserKey>
    {
        public Message()
        {
            this.ReadByUsers = new List<TUser>();
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime SendDate { get; set; }

        public string ChatRoomId { get; set; }

        public virtual ChatRoom<TUser, TUserKey> ChatRoom { get; set; }

        public TUserKey SenderId { get; set; }

        public virtual TUser Sender { get; set; }


        public IEnumerable<TUser> ReadByUsers { get; set; }
        public bool HasUnread { get; set; } = false;
    }
}
