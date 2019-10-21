using NoSocNet.BLL.Models;
using System.Collections.Generic;

namespace NoSocNet.Models
{
    public class IndexViewModel
    {
        public IEnumerable<UserViewModel> Users => UsersPage.Items;
        public PagedList<UserViewModel> UsersPage { get; set; }
        public IEnumerable<ChatRoomViewModel> Rooms { get; set; }
    }
}
