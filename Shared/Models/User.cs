namespace Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ImagePath { get; set; }

        public List<Chat> Chats { get; set; } = [];
    }
}
