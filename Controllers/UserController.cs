using Microsoft.AspNetCore.Mvc;
using rest1.Models;
using rest1.Services;
using System.Text.Json.Serialization;
using talkLib.Util;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/v1/user/")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> getUserList()
        {
            var userList = _userService.getUserList();
            return Ok(userList);
        }

        [HttpGet("{usrNo}")]
        public async Task<IActionResult> getUser(int usrNo)
        {
            var user = _userService.getUser(usrNo);
            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> create([FromBody] User user)
        {
            _userService.save(user);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> postLogin([FromBody] UserLoginRequestDto dto)
        {
            var user = _userService.login(dto.UsrId, dto.Password);
            return Ok(user);
        }

        [HttpPut("login")]
        public async Task<IActionResult> putLogin([FromBody] UserLoginRequestDto dto)
        {
            var user = _userService.login(dto.UsrId);
            return Ok(user);
        }

        [HttpPost("profile")]
        public async Task<IActionResult> postProfile([FromForm] int usrNo, [FromForm] IFormFile file)
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
            _userService.saveProfile(usrNo, f);
            return Ok();
        }

        [HttpDelete("profile")]
        public async Task<IActionResult> deleteProfile([FromBody] UserProfileDeleteRequestDto dto)
        {
            _userService.deleteProfile(dto.UsrNo);
            return Ok();
        }
    }

    public class UserLoginRequestDto
    {
        [JsonPropertyName("usrId")]
        public string UsrId { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    public class UserProfileDeleteRequestDto
    {
        [JsonPropertyName("usrNo")]
        public int UsrNo { get; set; }
    }
}
