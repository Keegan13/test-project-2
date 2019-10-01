using NoSocNet.BLL.Models;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using NoSocNet.Infrastructure.Services.Hub;
using NoSocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{
    public class HubMessageSender : HubBase, IMessageSender<User, string>
    {
        public Task<bool> Push(Message<User, string> message, IEnumerable<string> users)
        {
            try
            {
                MessageViewModel _message = new MessageViewModel
                {
                    ChatRoomId = message.ChatRoomId,
                    Id = message.Id,
                    SendDate = message.SendDate,
                    SenderId = message.SenderId,
                    Text = message.Text,
                    SenderUserName = message.Sender.UserName
                };

                base.Notify(_message, users.ToArray());

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }
    }
}
