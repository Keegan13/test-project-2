using System.Collections;
using System.Collections.Generic;

namespace NoSocNet.Models
{
    public class InviteUsersViewModel
    {
        public string RoomId { get; set; }

        public string RoomName { get; set; }

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
        public IEnumerable<UserViewModel> Users { get; set; }
    }
}
