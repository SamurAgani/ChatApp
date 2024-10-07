namespace Shared.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int UnreadMessages { get; set; }
        public List<Message> Messages { get; set; } = [];
        public User Receiver { get; set; }
        public User Sender { get; set; }
    }
}
