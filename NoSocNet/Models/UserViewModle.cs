using NoSocNet.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class UserViewModel
    {
        public virtual string PhoneNumber { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Id { get; set; }
        public virtual bool LockedOut { get; set; }
    }
}
