using FluentResults;
using Shared.Models;

namespace ChatApp.Services.Abstract
{
    public interface IChatRepo
    {
        // In IChatRepo
        Task<Result<User>> GetUserByUserNameAsync(string username);
        Task<Result> CreateUserAsync(User user);
        Task<Result<List<Chat>>> GetChatsByUserNameAsync(string username, int page, int pageSize);
        Task<Result> UpdateChatAsync(Chat chat);
        Task<Result<Chat>> GetChatBetweenUsersAsync(string senderName, string receiverName);

        Task<Result<List<Chat>>> GetAllChatsAsync();
        Task<Result> AddMessageAsync(Message message);
        Task<Result> CreateChatAsync(Chat chat);
        Task<Result> IncreaseUnreadMessagesAsync(int chatId);

    }
}
