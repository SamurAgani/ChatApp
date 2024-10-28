using ChatApp.Client.Services.Abstract;
using FluentResults;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.DTOs;
using Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;

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
            return result ? Result.Ok(true) : Result.Fail("User does not exist.");
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
            var response = await _httpClient.PostAsJsonAsync("api/User/GetOrCreateUser", userName);
            if (!response.IsSuccessStatusCode)
                return await HandleErrorResponse<User>(response);

            var user = await response.Content.ReadFromJsonAsync<User>();
            return user != null ? Result.Ok(user) : Result.Fail("User could not be created or retrieved.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving user: {ex.Message}");
        }
    }

    public async Task<Result<List<Chat>>> GetUserChatsAsync(string userName)
    {
        try
        {
            var chats = await _httpClient.GetFromJsonAsync<List<Chat>>($"api/user/GetUserChats?username={userName}");
            return chats != null ? Result.Ok(chats) : Result.Fail("No chats found.");
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
            var dto = new GetOrCreateChatDto { ReceiverName = receiverName, SenderName = senderName };
            var response = await _httpClient.PostAsJsonAsync("/api/chat/GetOrCreateChat", dto);

            if (!response.IsSuccessStatusCode)
                return await HandleErrorResponse<Chat>(response);

            var chat = await response.Content.ReadFromJsonAsync<Chat>();
            return chat != null ? Result.Ok(chat) : Result.Fail("Chat could not be created or retrieved.");
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
            var response = await _httpClient.PostAsJsonAsync("/api/chat/UpdateChat", chat);
            if (!response.IsSuccessStatusCode)
                return await HandleErrorResponse(response);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating chat: {ex.Message}");
        }
    }

    public async Task<Result<List<Chat>>> GetChatsByUserNameAsync(string username, int page, int pageSize)
    {
        try
        {
            var chats = await _httpClient.GetFromJsonAsync<List<Chat>>($"/api/User/GetUserChats?username={username}&page={page}&pageSize={pageSize}");
            return chats != null ? Result.Ok(chats) : Result.Fail("No chats found.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving chats: {ex.Message}");
        }
    }

    private async Task<Result<T>> HandleErrorResponse<T>(HttpResponseMessage response)
    {
        var errorMessage = await response.Content.ReadAsStringAsync();
        return Result.Fail($"Error: {errorMessage}");
    }

    private async Task<Result> HandleErrorResponse(HttpResponseMessage response)
    {
        var errorMessage = await response.Content.ReadAsStringAsync();
        return Result.Fail($"Error: {errorMessage}");
    }
}
