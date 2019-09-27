using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class HubResponseViewModel
    {
        public string ConnectionId { get; set; }

        public string Message { get; set; }

        public string CallBack { get; set; }
    }
}
