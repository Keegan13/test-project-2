using Microsoft.Extensions.Options;
using NoSocNet.BLL.Services;
using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services.Hub
{

    public class MessageObserverOptions
    {
        /// <summary>
        /// Milliseconds
        /// </summary>
        public int MaxTimeOut { get; set; }

        public int RefreshInterval { get; set; }
    }
    public class MessageObserver
    {
        public readonly IIdentityService<User> identity;
        private readonly HubMessageSender hub;

        #region Options
        protected virtual int MaxTimeOut { get; set; } = 1000 * 60 * 25;
        protected virtual int RefreshInterval { get; set; } = 1000;
        #endregion

        private Object message = null;

        public MessageObserver(
            IOptions<MessageObserverOptions> options,
            IMessageSender<User, string> hub,
            IIdentityService<User> identity
            )
        {
            this.identity = identity;
            this.hub = hub as HubMessageSender;
            this.ApplyOptions(options.Value);
        }

        private void ApplyOptions(MessageObserverOptions options)
        {
            if (options == null)
            {
                return;
            }

            if (options.MaxTimeOut > 0)
            {
                this.MaxTimeOut = options.MaxTimeOut;
            }

            if (options.RefreshInterval > 0)
            {
                this.RefreshInterval = options.RefreshInterval;
            }
        }

        public async Task<Object> GetMessageOrDefaultAsync(Guid connectionId, int? timeOut = null)
        {
            DateTime endDate = DateTime.Now.AddMilliseconds(timeOut.HasValue ? timeOut.Value : this.MaxTimeOut);

            if (!(this.hub.Connect(connectionId) is Connection connection))
            {
                return null;
            }

            var handler = new EventHandler<MessageEventArguments>(onMessageHandler);
            connection.onMessage += handler;

            try
            {
                do
                {
                    if (this.message != null)
                    {
                        return this.message;
                    }
                    await Task.Delay(1000);
                }
                while (DateTime.Now < endDate);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                connection.onMessage -= handler;
            }

            return this.message;
        }

        private void onMessageHandler(object sender, MessageEventArguments e)
        {
            this.message = e.Message;
        }
    }
}

