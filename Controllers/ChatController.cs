using Microsoft.AspNetCore.Mvc;
using rest1.Services;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/v1/chat/[controller]")]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("chatList")]
        public async Task<IActionResult> getChatList([FromBody] RequestDto dto)
        {
            var chatList = _chatService.getChatList(dto.usrNo);
            return Ok(chatList);
        }
    }

    public class RequestDto
    {
        public int usrNo { get; set; }
    }
}
