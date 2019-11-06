using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Interfaces;
using NoSocNet.Core.Models;
using NoSocNet.Extensions;
using NoSocNet.Infrastructure.Services;
using NoSocNet.Infrastructure.Services.Notificator;
using NoSocNet.Models;

namespace NoSocNet.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LiveController : ControllerBase
    {
        public readonly IIdentityService identity;
        private readonly NotificationService notificator;
        private readonly NotificationObserver observer;
        private readonly IMapper mapper;

        public LiveController(
            IMapper mapper,
            NotificationService notificator,
            IIdentityService identity,
            NotificationObserver observer
            )
        {
            this.observer = observer;
            this.mapper = mapper;
            this.identity = identity;
            this.notificator = notificator;
        }


        [HttpGet]
        public ContentResult Get()
        {
            string connectionId = null;

            if (HttpContext.Request.Cookies.TryGetValue("connectionId", out string existingConnection) && Guid.TryParse(existingConnection, out Guid connection) && this.notificator.Subscribe(connection, null))
            {
                connectionId = existingConnection;
            }
            else
            {
                connectionId = this.notificator.Connect(this.identity.CurrentUserId).ToString();
            }

            HttpContext.Response.Cookies.Append("connectionId", connectionId.ToString(), new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            });

            //if (mode == Mode.ForeverFrame)
            //{
            //    return await this.Put(model.ConnectionId.ToString());
            //}
            return new ContentResult
            {
                Content = $"<script src=\"/js/live.js\"></script><script>connectLive(\"/api/live\",\"{connectionId}\")</script>",
                ContentType = "text/html",
                StatusCode = 200
            };
        }

        //long pulling
        [HttpPost]
        public TypedNotification Post([FromForm] string connectionId)
        {
            if (!String.IsNullOrEmpty(connectionId))
            {
                TypedNotification notification = this.observer.GetMessageOrDefaultAsync(Guid.Parse(connectionId), 1000 * 60);

                if (notification == null)
                {
                    return null;
                }

                if (notification.Type == AppNotificationType.ChatJoin)
                {
                    var dto = notification.Notification as NewChatUser;
                    ChatJoinViewModel vm = mapper.Map<NewChatUser, ChatJoinViewModel>(dto);

                    return new TypedNotification(AppNotificationType.ChatJoin, vm);
                }

                if (notification.Type == AppNotificationType.Message)
                {
                    return new TypedNotification(AppNotificationType.Message, mapper.Map<MessageDto, MessageViewModel>(notification.Notification as MessageDto));
                }

                if (notification.Type == AppNotificationType.NewChat)
                {
                    var dto = notification.Notification as ChatRoomDto;
                    ChatRoomViewModel vm = mapper.Map<ChatRoomDto, ChatRoomViewModel>(dto);
                    vm.RoomName = dto.GetRoomName(identity.CurrentUserId);

                    return new TypedNotification(AppNotificationType.NewChat, vm);
                }


                return notification;
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
        public async Task<FileStreamResult> Put(string connecntionId)
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
