using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Models;
using NoSocNet.Domain.Interfaces;
using NoSocNet.Domain.Models;
using NoSocNet.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NoSocNet.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository messsages;
        private readonly IIdentityService identity;
        private readonly IChatService chatService;
        private readonly IMapper mapper;
        public MessagesController(
            IMessageRepository repo,
            IIdentityService identity,
            IChatService chatService,
            IMapper mapper
            )
        {
            this.chatService = chatService;
            this.mapper = mapper;
            this.messsages = repo;
            
            this.identity = identity;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Sent(string text, string roomId)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Ok();
            }

            if (await this.chatService.AddMessage(this.identity.CurrentUserId, roomId, text) is MessageDto message)
            {
                return new JsonResult(new MessageViewModel
                {
                    ChatRoomId = message.ChatRoomId,
                    Id = message.Id,
                    SendDate = message.SendDate,
                    SenderId = message.Sender.Id,
                    Text = message.Text,
                    SenderUserName = message.Sender.UserName
                });
            }

            return NotFound(new { roomId });
        }


        [HttpGet]
        public async Task<MessageViewModel[]> Get(string roomId, int? tail)
        {
            var messages = await messsages.GetMessagesAsync(roomId, 10, tail);

            return messages.Select(x => mapper.Map<MessageEntity, MessageViewModel>(x)).ToArray();
        }
    }
}
