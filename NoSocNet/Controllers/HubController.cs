using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Interfaces;
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
        public readonly IIdentityService identity;
        private readonly ApplicationNotificator notificator;
        private readonly MessageObserver observer;

        public HubController(
            ApplicationNotificator notificator,
            IIdentityService identity,
            MessageObserver observer
            )
        {
            this.observer = observer;
            this.identity = identity;
            this.notificator = notificator;
        }


        [HttpGet]
        [ActionName("Index")]
        public IActionResult Get(Mode? mode = null)
        {
            var model = new HubResponseViewModel();

            if (HttpContext.Request.Cookies.TryGetValue("connectionId", out string existingConnection) && Guid.TryParse(existingConnection, out Guid connection) && this.notificator.Subscribe(connection, null))
            {
                model.ConnectionId = existingConnection;
            }
            else
            {
                model.ConnectionId = this.notificator.Connect(this.identity.CurrentUserId).ToString();
            }

            HttpContext.Response.Cookies.Append("connectionId", model.ConnectionId.ToString(), new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            });

            //if (mode == Mode.ForeverFrame)
            //{
            //    return await this.Put(model.ConnectionId.ToString());
            //}

            return PartialView("_HiddenFrame", model);
        }


        //long pulling
        [HttpPost]
        [ActionName("Index")]
        public IActionResult Post(string connectionId)
        {
            if (!String.IsNullOrEmpty(connectionId))
            {
                var message = this.observer.GetMessageOrDefaultAsync(Guid.Parse(connectionId), 1000 * 60);

                return new JsonResult(message);
            }
            return null;
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
            HttpContext.Response.ContentType = "text/html";

            using (var writer = new StreamWriter(body))
            {
                await writer.WriteAsync("<script>function callback(message) {parent._onMessageReceived(message);}</script>");
                await writer.FlushAsync();
                while (true)
                {
                    var message = this.observer.GetMessageOrDefaultAsync(Guid.Parse(connecntionId));
                    if (message != null)
                    {
                        await writer.WriteAsync(WrapMessageInScript(message).Value);
                        await writer.FlushAsync();
                    }
                }
            }
        }
    }
}
