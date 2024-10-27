using ChatApp.Services.Abstract;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace ChatApp.Services.Concrete
{
    public class ChatRepo : IChatRepo
    {
        private readonly AppDbContext _dbContext;

        public ChatRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Chat>>> GetAllChatsAsync()
        {
            try
            {
                var chats = await _dbContext.Chats.ToListAsync();
                return Result.Ok(chats);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while fetching all chats").CausedBy(ex));
            }
        }

        public async Task<Result> AddMessageAsync(Message message)
        {
            try
            {
                _dbContext.Messages.Add(message);
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while adding the message").CausedBy(ex));
            }
        }

        public async Task<Result<List<Chat>>> GetChatsByUserNameAsync(string username, int page, int pageSize)
        {
            try
            {
                var chats = await _dbContext.Chats
                    .Include(x => x.Sender)
                    .Include(x => x.Receiver)
                    .Include(x => x.Messages)
                    .Where(c => c.Receiver.UserName == username || c.Sender.UserName == username)
                    .OrderByDescending(c => c.Messages.Max(m => m.TimeSent))
                    .ThenByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                foreach (var chat in chats)
                {
                    var lastMessage = chat.Messages.OrderByDescending(m => m.TimeSent).FirstOrDefault();

                    if (lastMessage != null && lastMessage.SenderName == username)
                    {
                        chat.UnreadMessages = 0;
                    }
                }

                return Result.Ok(chats);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while fetching chats by username").CausedBy(ex));
            }
        }

        public async Task<Result<User>> GetUserByUserNameAsync(string username)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null)
                {
                    return Result.Fail(new Error("User not found"));
                }

                return Result.Ok(user);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while fetching the user").CausedBy(ex));
            }
        }

        public async Task<Result> CreateUserAsync(User user)
        {
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while creating the user").CausedBy(ex));
            }
        }

        public async Task<Result<Chat>> GetChatBetweenUsersAsync(string senderName, string receiverName)
        {
            try
            {
                var chat = await _dbContext.Chats
                    .Include(x => x.Sender)
                    .Include(x => x.Receiver)
                    .Include(x => x.Messages)
                    .FirstOrDefaultAsync(c =>
                        (c.Sender.UserName == senderName && c.Receiver.UserName == receiverName) ||
                        (c.Sender.UserName == receiverName && c.Receiver.UserName == senderName));

                if (chat == null)
                {
                    return Result.Fail(new Error("Chat not found between the specified users"));
                }

                return Result.Ok(chat);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while fetching the chat").CausedBy(ex));
            }
        }

        public async Task<Result> CreateChatAsync(Chat chat)
        {
            try
            {
                _dbContext.Chats.Add(chat);
                await _dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while creating the chat").CausedBy(ex));
            }
        }

        public async Task<Result> IncreaseUnreadMessagesAsync(int chatId)
        {
            try
            {
                var chat = await _dbContext.Chats.FindAsync(chatId);

                if (chat == null)
                {
                    return Result.Fail(new Error("Chat not found"));
                }

                chat.UnreadMessages++;
                await _dbContext.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while increasing unread messages").CausedBy(ex));
            }
        }

        public async Task<Result> UpdateChatAsync(Chat chat)
        {
            try
            {
                var existingChat = await _dbContext.Chats.FindAsync(chat.Id);
                if (existingChat == null)
                {
                    return Result.Fail(new Error("Chat not found"));
                }

                existingChat.UnreadMessages = chat.UnreadMessages;

                _dbContext.Entry(existingChat).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while updating the chat").CausedBy(ex));
            }
        }

    }
}
