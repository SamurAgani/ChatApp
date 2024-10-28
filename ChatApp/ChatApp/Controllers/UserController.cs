using ChatApp.Services.Abstract;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

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
        public async Task<IActionResult> GetUserChats([FromQuery] string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(Result.Fail("Username is required."));

            var result = await _chatRepo.GetChatsByUserNameAsync(username, page, pageSize);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Errors.FirstOrDefault()?.Message);
        }

        [HttpPost("GetOrCreateUser")]
        public async Task<IActionResult> GetOrCreateUser([FromBody] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(Result.Fail("Username is required."));

            var userResult = await _chatRepo.GetUserByUserNameAsync(username);

            if (userResult.IsSuccess && userResult.Value != null)
                return Ok(userResult.Value);

            if (!userResult.IsSuccess)
                return StatusCode(500, userResult.Errors.FirstOrDefault()?.Message);

            var newUser = new User
            {
                UserName = username,
                ImagePath = "default.png"
            };

            var createResult = await _chatRepo.CreateUserAsync(newUser);

            return createResult.IsSuccess
                ? Ok(newUser)
                : StatusCode(500, createResult.Errors.FirstOrDefault()?.Message);
        }

        [HttpGet("UserExists")]
        public async Task<IActionResult> UserExists([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(Result.Fail("Username is required."));

            var userResult = await _chatRepo.GetUserByUserNameAsync(username);

            if (!userResult.IsSuccess)
                return StatusCode(500, userResult.Errors.FirstOrDefault()?.Message);

            return Ok(userResult.Value != null);
        }
    }
}
