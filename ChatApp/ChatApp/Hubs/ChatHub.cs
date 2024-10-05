using ChatApp.Services.Abstract;
using ChatApp.Services.Concrete;
using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using System.Collections.Concurrent;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public IChatRepo chatRepo { get; set; }
        private static ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        public ChatHub(IChatRepo chatRepo)
        {
            this.chatRepo = chatRepo;
        }

        public async Task Register(string userName)
        {
            var connectionId = Context.ConnectionId;

            _userConnections.AddOrUpdate(userName, connectionId, (key, oldValue) => connectionId);
            
            await Clients.Client(connectionId).SendAsync("Registered", "User successfully registered.");
        }

        public async Task SendMessage(Message message, string receiverName)
        {
            await chatRepo.AddMessageAsync(message);

            if (_userConnections.TryGetValue(receiverName, out string receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
            }
            else
            {
                await chatRepo.IncreaseUnreadMessagesAsync(message.ChatId);
            }
        }
    }
}
