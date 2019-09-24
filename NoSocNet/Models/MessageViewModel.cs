using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string ChatRoomId { get; set; }
        public DateTime SendDate { get; set; }

        public string SenderId { get; set; }

        public string SenderUserName { get; set; }
    }
}
