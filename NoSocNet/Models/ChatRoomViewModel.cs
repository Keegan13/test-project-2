using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using NoSocNet.BLL.Models;
using NoSocNet.DAL.Models;
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

        public UserViewModel Owner { get; set; }

        public string OwnerId { get; set; }

        public string Id { get; set; }
    }
}
