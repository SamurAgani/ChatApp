using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string SenderName { get; set; }
        public DateTime TimeSent { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
