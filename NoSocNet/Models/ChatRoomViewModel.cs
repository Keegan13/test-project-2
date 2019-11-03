using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class ChatRoomViewModel
    {
        public IList<MessageViewModel> Messages { get; set; }

        public IList<UserViewModel> Participants { get; set; }

        public bool IsPrivate { get; set; }

        public string RoomName { get; set; }

        public UserViewModel Owner { get; set; }
        public bool IsFocus { get; set; }

        public string OwnerId { get; set; }

        public string Id { get; set; }
        public bool HasUnread { get; set; } = false;

        ////WARNING HACK
        //public string RenderedTab { get; set; }

        //public string RenderedTabLink { get; set; }
    }
}
