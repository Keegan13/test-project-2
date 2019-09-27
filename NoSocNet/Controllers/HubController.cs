using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using NoSocNet.Infrastructure.Services;
using NoSocNet.Infrastructure.Services.Hub;
using NoSocNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoSocNet.Controllers
{
    [Authorize]
    public class HubController : Controller
    {
        public readonly IIdentityService<User> identity;
        private readonly HubMessageSender hub;
        private readonly MessageObserver observer;

        private Object message = null;

        public HubController(
            IMessageSender<User, string> hub,
            IIdentityService<User> identity,
            MessageObserver observer
            )
        {
            this.observer = observer;
            this.identity = identity;
            this.hub = hub as HubMessageSender;
        }


        [HttpGet]
        [ActionName("Index")]
        public IActionResult Get()
        {
            var connection = this.hub.Connect(this.identity.CurrentUserId);
            return PartialView("_HiddenFrame", new HubResponseViewModel
            {
                ConnectionId = connection.ConnectionId.ToString(),
            });
        }


        //long pulling
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> Post(string connectionId)
        {
            var message = await this.observer.GetMessageOrDefaultAsync(Guid.Parse(connectionId), 1000 * 60);

            return new JsonResult(message);
        }


        private HtmlString WrapMessageInScript(Object message)
        {
            var argument = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            var callFunc = $"callback({argument})";
            return new HtmlString($"<script>{callFunc}</srcipt>");
        }

        //Forever frame
        [HttpPut]
        [ActionName("Index")]
        public async Task<IActionResult> Put(string connecntionId)
        {
            var body = HttpContext.Response.Body;

            using (var writer = new StreamWriter(body))
            {
                while (true)
                {
                    var message = await this.observer.GetMessageOrDefaultAsync(Guid.Parse(connecntionId));
                    if (message != null)
                    {

                        await writer.WriteAsync(WrapMessageInScript(message).Value);
                        writer.Flush();
                    }
                }
            }
            return null;
        }
    }
}
