using Microsoft.AspNetCore.Mvc;
using rest1.Services;
using talkLib.Util;

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

        [HttpGet("list")]
        public async Task<IActionResult> getChatList([FromBody] RequestDto dto)
        {
            var chatList = _chatService.getChatList(dto.usrNo);
            return Ok(chatList);
        }

        [HttpGet("read/{roomNo}")]
        public async Task<IActionResult> readChat(int roomNo, [FromQuery] int usrNo)
        {
            var result = _chatService.ReadChat(roomNo, usrNo);
            return Ok(result);
        }

        [HttpPost("insert")]
        public async Task<IActionResult> insertChat([FromBody] int roomNo, [FromBody] int usrNo, [FromBody] string type, [FromBody] string msg)
        {
            var result = _chatService.InsertChat(roomNo, usrNo, type, msg);
            return Ok(result);
        }

        [HttpPost("insert1")]
        public async Task<IActionResult> insert1Chat([FromForm] int roomNo, [FromForm] int usrNo, [FromForm] string type, [FromForm] IFormFile file)
        {
            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            Models.File f = new Models.File
            {
                FilePath = $"D:/temp/{DateUtil.now("yyyyMMdd")}/",
                FileName = $"OTI{DateUtil.now("yyyyMMddHHmmddfffffff")}_{NumberUtil.random(1, 999)}",
                FileExt = Path.GetExtension(file.FileName),
                OriginName = file.FileName,
                Buffer = memoryStream.ToArray(),
            };
            var result = _chatService.InsertChat(roomNo, usrNo, type, f);
            return Ok(result);
        }

        /* [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            [FromForm] string userName,
            [FromForm] int age,
            [FromForm] bool isAdmin,
            [FromForm] IFormFile file)
        {
            // 데이터와 파일 모두 접근 가능
            return Ok(new { message = $"업로드 완료 - {userName}, {age}, {isAdmin}" });
        }*/
        /* [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFormModel model)
        {
            // model.UserName, model.File 등 사용 가능
            return Ok(new { message = "업로드 성공" });
        } */

    }

    public class RequestDto
    {
        public int roomNo { get; set; }
        public int usrNo { get; set; }
        public string type { get; set; }
        public string msg { get; set; }
    }
}
