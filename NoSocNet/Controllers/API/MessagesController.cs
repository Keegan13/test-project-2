using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.DAL.Models;
using NoSocNet.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoSocNet.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository<User, string> repo;
        public MessagesController(
            IMessageRepository<User, string> repo
            )
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<MessageViewModel[]> Get(string roomId, int? tail)
        {
            var messages = await repo.GetMessagesAsync(roomId, 10, tail);

            return messages.Select(x => new MessageViewModel
            {
                Id = x.Id,
                ChatRoomId = x.ChatRoomId,
                ReadByUsersIds = x.ReadByUsers.Select(u => u.Id).ToList(),
                SendDate = x.SendDate,
                SenderId = x.SenderId,
                Text = x.Text,
                SenderUserName = x.Sender.UserName
            }).ToArray();
        }
    }
}
