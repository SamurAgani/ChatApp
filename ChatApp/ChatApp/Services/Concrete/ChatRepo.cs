using ChatApp.Services.Abstract;
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

        public async Task<List<Chat>> GetAllChatsAsync()
        {
            return await _dbContext.Chats
                .Include(c => c.Messages)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .ToListAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Chat>> GetChatsByUserNameAsync(string username)
        {
            var chats = await _dbContext.Chats
                        .Include(c => c.Messages)
                        .Include(c => c.Sender)
                        .Include(c => c.Receiver)
                        .Where(c => c.Receiver.UserName == username || c.Sender.UserName == username)
                        .ToListAsync();

            foreach (var chat in chats)
            {
                var lastMessage = chat.Messages.OrderByDescending(m => m.TimeSent).FirstOrDefault();

                if (lastMessage != null && lastMessage.SenderName == username)
                {
                    chat.UnreadMessages = 0;
                }
            }

            return chats;
        }

        public async Task<User?> GetUserByUserNameAsync(string username)
        {
            return await _dbContext.Users.Include(c => c.Chats).FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task CreateUserAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Chat?> GetChatBetweenUsersAsync(string senderName, string receiverName)
        {
            return await _dbContext.Chats
                .Include(c => c.Messages)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .FirstOrDefaultAsync(c =>
                    (c.Sender.UserName == senderName && c.Receiver.UserName == receiverName) ||
                    (c.Sender.UserName == receiverName && c.Receiver.UserName == senderName));
        }

        public async Task CreateChatAsync(Chat chat)
        {
            _dbContext.Chats.Add(chat);
            await _dbContext.SaveChangesAsync();
        }

        public async Task IncreaseUnreadMessagesAsync(int chatId)
        {
            var chat = await _dbContext.Chats.FindAsync(chatId);

            if (chat == null)
            {
                return;
            }

            chat.UnreadMessages++;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChatAsync(Chat chat)
        {
            var existingChat = await _dbContext.Chats.FindAsync(chat.Id);
            if (existingChat != null)
            {
                existingChat.UnreadMessages = chat.UnreadMessages;

                _dbContext.Entry(existingChat).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
