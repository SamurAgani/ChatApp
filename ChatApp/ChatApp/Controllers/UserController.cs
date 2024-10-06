using ChatApp.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IChatRepo _chatRepo;
        public UserController(IChatRepo chatRepo)
        {
            _chatRepo = chatRepo;
        }

        [HttpGet("GetUserChats")]
        public async Task<ActionResult<List<Chat>>> GetUserChats([FromQuery] string username)
        {
            var Chats = await _chatRepo.GetChatsByUserNameAsync(username);

            return Ok(Chats);
        }
        [HttpPost("UpdateChat")]
        public async Task<ActionResult> UpdateChat(Chat chat)
        {
            await _chatRepo.UpdateChatAsync(chat);
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult<User>> GetOrCreateUser([FromQuery] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username is required.");
                }

                var user = await _chatRepo.GetUserByUserNameAsync(username);

                if (user == null)
                {
                    var newUser = new User
                    {
                        UserName = username,
                        ImagePath = "default.png"
                    };

                    await _chatRepo.CreateUserAsync(newUser);
                    user = newUser; // Now the user exists
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("UserExists")]
        public async Task<ActionResult<bool>> UserExists([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }

            var user = await _chatRepo.GetUserByUserNameAsync(username);
            return Ok(user != null);
        }

        [HttpGet("GetOrCreateChat")]
        public async Task<ActionResult<Chat>> GetOrCreateChat([FromQuery] string senderName, [FromQuery] string receiverName)
        {
            if (string.IsNullOrEmpty(senderName) || string.IsNullOrEmpty(receiverName))
            {
                return BadRequest("Both senderName and receiverName are required.");
            }

            // Check if both users exist
            var sender = await _chatRepo.GetUserByUserNameAsync(senderName);
            var receiver = await _chatRepo.GetUserByUserNameAsync(receiverName);

            if (sender == null || receiver == null)
            {
                return NotFound("One or both users do not exist.");
            }

            // Check if a chat exists between sender and receiver
            var existingChat = await _chatRepo.GetChatBetweenUsersAsync(senderName, receiverName);

            if (existingChat != null)
            {
                if (existingChat.Messages.OrderByDescending(x => x.Id).FirstOrDefault()?.SenderName != senderName)
                {
                    existingChat.UnreadMessages = 0;
                    await _chatRepo.UpdateChatAsync(existingChat);
                }
                return Ok(existingChat);
            }

            // If no existing chat, create a new one
            var newChat = new Chat
            {
                Sender = sender,
                Receiver = receiver,
                Messages = new List<Message>()
            };

            await _chatRepo.CreateChatAsync(newChat);

            return CreatedAtAction(nameof(GetOrCreateChat), new { senderName, receiverName }, newChat);
        }
    }
}
