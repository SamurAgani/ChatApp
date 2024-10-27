using FluentResults;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Models;

namespace ChatApp.Client.Services.Abstract
{
    public interface IChatService
    {
        Task<Result<bool>> UserExistsAsync(string userName);
        Task<Result<User>> GetUserAsync(string userName);
        Task<Result<List<Chat>>> GetUserChatsAsync(string userName);
        Task<Result> RegisterUserAsync(HubConnection hubConnection, string userName);
        Task<Result<Chat>> GetOrCreateChatAsync(string senderName, string receiverName); 
        Task<Result> UpdateChatAsync(Chat chat);
        Task<Result<List<Chat>>> GetChatsByUserNameAsync(string username, int page, int pageSize);
    }
}
