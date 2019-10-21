using NoSocNet.BLL.Enums;

namespace NoSocNet.BLL.Models
{
    public class FilterBase
    {
        public string Keywords { get; set; }

        public string ChatRoomId { get; set; }

        public ParticipantsType Type { get; set; }

        public string CurrentUserId { get; set; }
    }
}
