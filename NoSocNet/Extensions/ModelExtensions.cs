using NoSocNet.Core.Models;
using NoSocNet.Models;
using System;
using System.Linq;

namespace NoSocNet.Extensions
{
    public static class ModelExtensions
    {
        public static string GetRoomName(this ChatRoomViewModel room, string currentUserId)
        {
            if (!String.IsNullOrWhiteSpace(room.RoomName))
            {
                return room.RoomName;
            }

            if (room.IsPrivate)
            {
                var name = room.Participants.FirstOrDefault(x => x.Id != currentUserId)?.UserName;

                if (String.IsNullOrWhiteSpace(name))
                {
                    name = "Empty Chat";
                }

                return name;
            }

            return String.Format("# ({1}) {0}", room.Participants.Where(x => x.Id != currentUserId).Aggregate("", (agg, next) => agg += next.UserName + ", ").Trim(' ', ','), room.Participants.Count());
        }

        public static string GetRoomName(this ChatRoomDto room, string currentUserId)
        {
            if (!String.IsNullOrWhiteSpace(room.RoomName))
            {
                return room.RoomName;
            }

            if (room.IsPrivate)
            {
                var name = room.Participants.FirstOrDefault(x => x.Id != currentUserId)?.UserName;

                if (String.IsNullOrWhiteSpace(name))
                {
                    name = "Empty Chat";
                }

                return name;
            }

            return String.Format("# ({1}) {0}", room.Participants.Where(x => x.Id != currentUserId).Aggregate("", (agg, next) => agg += next.UserName + ", ").Trim(' ', ','), room.Participants.Count());
        }

    }
}
