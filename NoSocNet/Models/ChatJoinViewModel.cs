using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class ChatJoinViewModel
    {
        public string ChatName { get; set; }
        public string ChatId { get; set; }
        public UserViewModel User { get; set; }
    }
}
