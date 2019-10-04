using NoSocNet.BLL.Models;
using NoSocNet.DAL.Models;
using NoSocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Extensions
{
    public static class ModelExtensions
    {
        public static string GetRoomName(this ChatRoomViewModel room, string currentUserId)
        {
            if (!String.IsNullOrEmpty(room.RoomName))
            {
                return room.RoomName;
            }

            if (room.IsPrivate)
            {
                return room.Participants.FirstOrDefault(x => x.Id != currentUserId)?.UserName;
            }

            return room.Participants.Where(x => x.Id != currentUserId).Aggregate("# ", (agg, next) => agg += next.UserName + ", ").Trim(' ', ',');
        }

        public static string GetRoomName(this ChatRoom<User, string> room, string currentUserId)
        {
            if (!String.IsNullOrEmpty(room.RoomName))
            {
                return room.RoomName;
            }

            if (room.IsPrivate)
            {
                return room.Participants.FirstOrDefault(x => x.Id != currentUserId)?.UserName;
            }

            return room.Participants.Where(x => x.Id != currentUserId).Aggregate("# ", (agg, next) => agg += next.UserName + ", ").Trim(' ', ',');
        }

    }
}
