﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class UserFilterModel : PaginationModel
    {
        public string Keywords { get; set; }

        public string Subject { get; set; }

        public string ChatRoomId { get; set; }

        public NoSocNet.BLL.Enums.ParticipantsType Type { get; set; }
    }
}
