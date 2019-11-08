using System.ComponentModel.DataAnnotations;

namespace NoSocNet.Models.API
{
    public class MessageInput
    {
        public string Text { get; set; }

        [Required]
        [RegularExpression("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}")]
        public string ChatRoomId { get; set; }

        public int? SurveyId { get; set; }
    }
}
