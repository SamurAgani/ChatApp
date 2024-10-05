using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int UnreadMessages { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public User Receiver { get; set; }
        public User Sender { get; set; }
    }
}
