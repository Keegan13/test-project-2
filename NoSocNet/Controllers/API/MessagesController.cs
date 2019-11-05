using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Models;
using NoSocNet.Core.Interfaces;
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

        public class InputModel
        {
            public string Text { get; set; }
            public string ChatRoomId { get; set; }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Sent([FromBody] InputModel input)
        {
            if (string.IsNullOrWhiteSpace(input.Text))
            {
                return Ok();
            }

            if (await this.chatService.AddMessage(this.identity.CurrentUserId, input.ChatRoomId, input.Text) is MessageDto message)
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

            return NotFound(new { input.ChatRoomId });
        }


        [HttpGet]
        public async Task<MessageViewModel[]> Get(string chatRoomid, int? tailMessageId)
        {
            var messages = await messsages.GetMessagesAsync(chatRoomid, 10, tailMessageId);

            return messages.Select(x => mapper.Map<MessageEntity, MessageViewModel>(x)).ToArray();
        }
    }
}
