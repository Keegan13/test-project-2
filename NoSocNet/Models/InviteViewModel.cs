using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class InviteViewModel
    {
        public string RoomId { get; set; }

        public string RoomName { get; set; }

        public IEnumerable<UserViewModel> Users { get; set; }
    }
}
