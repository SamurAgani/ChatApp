using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace ChatApp.Services.Abstract
{
    public interface IChatRepo
    {
        public Task<List<Shared.Models.Chat>> GetAllChatsAsync();
        public Task AddMessageAsync(Message message);
        public Task<IEnumerable<Chat>> GetChatsByUserNameAsync(string username); 
        Task<User?> GetUserByUserNameAsync(string username);
        Task CreateUserAsync(User user);
        Task<Chat?> GetChatBetweenUsersAsync(string senderName, string receiverName);
        Task CreateChatAsync(Chat chat);
        Task IncreaseUnreadMessagesAsync(int chatId);
        Task UpdateChatAsync(Chat chat);

    }
}
