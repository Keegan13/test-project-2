using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class UserSearchResult
    {
        public IEnumerable<UserViewModel> Users { get; set; }
    }

    public class SearchResult
    { 
        public IEnumerable<MessageViewModel> Messages { get; set; }

        public IEnumerable<UserViewModel> Users { get; set; }

        public IEnumerable<ChatRoomViewModel> ChatRooms { get; set; }
    }
}
