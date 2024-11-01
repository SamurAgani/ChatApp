﻿using ChatApp.Services.Abstract;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepo _chatRepo;

        public ChatController(IChatRepo chatRepo)
        {
            _chatRepo = chatRepo;
        }

        [HttpPost("GetOrCreateChat")]
        public async Task<IActionResult> GetOrCreateChat([FromBody] GetOrCreateChatDto getOrCreateChatDto)
        {
            if (string.IsNullOrWhiteSpace(getOrCreateChatDto.SenderName) || string.IsNullOrWhiteSpace(getOrCreateChatDto.ReceiverName))
                return BadRequest(Result.Fail("Both senderName and receiverName are required."));

            var senderResult = await _chatRepo.GetUserByUserNameAsync(getOrCreateChatDto.SenderName);
            var receiverResult = await _chatRepo.GetUserByUserNameAsync(getOrCreateChatDto.ReceiverName);

            if (senderResult.IsFailed || senderResult.Value == null || receiverResult.IsFailed || receiverResult.Value == null)
                return NotFound(Result.Fail("One or both users do not exist."));

            var chatResult = await _chatRepo.GetChatBetweenUsersAsync(getOrCreateChatDto.SenderName, getOrCreateChatDto.ReceiverName);

            if (chatResult.IsSuccess && chatResult.Value != null)
            {
                await UpdateUnreadMessagesAsync(chatResult.Value, getOrCreateChatDto.SenderName);
                return Ok(chatResult.Value);
            }

            var newChat = new Chat
            {
                Sender = senderResult.Value,
                Receiver = receiverResult.Value,
                Messages = new List<Message>()
            };

            var createChatResult = await _chatRepo.CreateChatAsync(newChat);

            return createChatResult.IsSuccess
                ? CreatedAtAction(nameof(GetOrCreateChat), new { getOrCreateChatDto.SenderName, getOrCreateChatDto.ReceiverName }, newChat)
                : StatusCode(500, createChatResult.Errors.FirstOrDefault()?.Message);
        }

        [HttpPost("UpdateChat")]
        public async Task<IActionResult> UpdateChat([FromBody] Chat chat)
        {
            if (chat == null)
                return BadRequest(Result.Fail("Chat data is required."));

            var result = await _chatRepo.UpdateChatAsync(chat);

            return result.IsSuccess
                ? Ok()
                : BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        private async Task UpdateUnreadMessagesAsync(Chat existingChat, string senderName)
        {
            var lastMessage = existingChat.Messages.OrderByDescending(x => x.Id).FirstOrDefault();

            if (lastMessage?.SenderName != senderName)
            {
                existingChat.UnreadMessages = 0;
                await _chatRepo.UpdateChatAsync(existingChat);
            }
        }
    }
}
