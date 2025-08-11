using Microsoft.AspNetCore.Mvc;
using rest1.Models;
using rest1.Services;
using System.Text.Json.Serialization;

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
        public async Task<IActionResult> login([FromBody] UserLoginRequestDto dto)
        {
            var user = _userService.login(dto.UsrId, dto.Password);
            return Ok(user);
        }
    }

    public class UserLoginRequestDto
    {
        [JsonPropertyName("usrId")]
        public String UsrId { get; set; }
        [JsonPropertyName("password")]
        public String Password { get; set; }
    }
}
