using System.Collections.Generic;

namespace NoSocNet.Models
{
    public class IndexViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
        public IEnumerable<ChatRoomViewModel> Rooms { get; set; }
    }
}
