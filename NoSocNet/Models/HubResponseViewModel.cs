using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{

    public enum Mode
    {
        LongPooling,
        ForeverFrame
    }
    public class HubResponseViewModel
    {
        public string ConnectionId { get; set; }

        public Mode Mode { get; set; } = Mode.LongPooling;

        public string Message { get; set; }

        public string CallBack { get; set; }
    }
}
