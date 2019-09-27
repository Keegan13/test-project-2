using NoSocNet.BLL.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Services
{
    public interface IMessageSender<TUser, TKey>
    {
        Task<bool> Push(Message<TUser, TKey> message, IEnumerable<string> users);
    }
}
