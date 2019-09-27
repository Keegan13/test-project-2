using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{
    public class MessageEventArguments
    {
        public MessageEventArguments(object message)
        {
            this.Message = message;
        }

        public object Message { get; }
    }
}
