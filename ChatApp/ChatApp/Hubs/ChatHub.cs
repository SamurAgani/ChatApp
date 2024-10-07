using ChatApp.Services.Abstract;
using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using System.Collections.Concurrent;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private IChatRepo _chatRepo { get; set; }
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();

        public ChatHub(IChatRepo chatRepo)
        {
            _chatRepo = chatRepo;
        }

        public async Task Register(string userName)
        {
            var connectionId = Context.ConnectionId;

            _userConnections.AddOrUpdate(userName, connectionId, (key, oldValue) => connectionId);
            
            await Clients.Client(connectionId).SendAsync("Registered", "User successfully registered.");
        }

        public async Task SendMessage(Message message, string receiverName)
        {
            await _chatRepo.AddMessageAsync(message);

            if (_userConnections.TryGetValue(receiverName, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
                return;
            }

            await _chatRepo.IncreaseUnreadMessagesAsync(message.ChatId);
        }
    }
}
