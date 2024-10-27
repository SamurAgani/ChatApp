using ChatApp.Client.Services.Abstract;
using FluentResults;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.DTOs;
using Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ChatApp.Client.Services.Concrete;
public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<bool>> UserExistsAsync(string userName)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<bool>($"api/user/UserExists?username={userName}");
            return result ? Result.Ok(result) : Result.Fail("User does not exist.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error checking user: {ex.Message}");
        }
    }
    public async Task<Result<User>> GetUserAsync(string userName)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/User/GetOrCreateUser", userName);
            if (response.IsSuccessStatusCode)
            {
                var User = await response.Content.ReadFromJsonAsync<User>();
                return User != null ? Result.Ok(User) : Result.Fail("Error: chat could not be created or retrieved.");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Result.Fail($"Error retrieving chat: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving chat: {ex.Message}");
        }
    }
    public async Task<Result<List<Chat>>> GetUserChatsAsync(string userName)
    {
        try
        {
            var chats = await _httpClient.GetFromJsonAsync<List<Chat>>($"api/user/GetUserChats?username={userName}");
            return chats != null ? Result.Ok(chats) : Result.Fail("No chats found");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving chats: {ex.Message}");
        }
    }
    public async Task<Result> RegisterUserAsync(HubConnection hubConnection, string userName)
    {
        try
        {
            await hubConnection.InvokeAsync("Register", userName);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error registering user: {ex.Message}");
        }
    }
    public async Task<Result<Chat>> GetOrCreateChatAsync(string senderName, string receiverName)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/chat/GetOrCreateChat", new GetOrCreateChatDto { ReceiverName = receiverName, SenderName = senderName });

            if (response.IsSuccessStatusCode)
            {
                var chat = await response.Content.ReadFromJsonAsync<Chat>();
                return chat != null ? Result.Ok(chat) : Result.Fail("Error: chat could not be created or retrieved.");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Result.Fail($"Error retrieving chat: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving chat: {ex.Message}");
        }
    }
    public async Task<Result> UpdateChatAsync(Chat chat)
    {
        try
        {
            await _httpClient.PostAsJsonAsync("/api/chat/UpdateChat", chat);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating chat: {ex.Message}");
        }
    }

    public async Task<Result<List<Chat>>> GetChatsByUserNameAsync(string username, int page, int pageSize)
    {
        var response = await _httpClient.GetFromJsonAsync<List<Chat>>($"/api/User/GetUserChats?username={username}&page={page}&pageSize={pageSize}");
        return Result.Ok(response);

    }
}