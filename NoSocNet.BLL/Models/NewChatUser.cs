namespace NoSocNet.BLL.Models
{
    public class NewChatUser<TUser, TKey>
    {
        public ChatRoom<TUser, TKey> Room { get; set; }

        public TUser User { get; set; }
    }
}
