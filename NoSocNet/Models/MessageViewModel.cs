using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class MessageViewModel
    {
        private string text;

        public MessageViewModel()
        {
            this.ReadByUsersIds = new List<string>();
        }
        public int Id { get; set; }

        public string Text
        {
            get
            {
                return HtmlEncoder.Default.Encode(this.text);
            }
            set
            {
                this.text = value;
            }
        }

        public string ChatRoomId { get; set; }

        public DateTime SendDate { get; set; }

        public string SenderId { get; set; }

        public string SenderUserName { get; set; }

        public List<string> ReadByUsersIds { get; set; }
    }
}
