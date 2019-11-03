using System;
using System.Collections.Generic;
using System.Text;

namespace NoSocNet.Domain.Interfaces
{
    public interface IIdentifiable<TKey>
    {
        TKey Id { get; set; }
    }
}
