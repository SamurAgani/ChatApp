using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class GetOrCreateChatDto
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
    }
}
