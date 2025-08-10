using Microsoft.AspNetCore.Mvc;
using rest1.Services;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/v1/user/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("userList")]
        public async Task<IActionResult> getChatList()
        {
            var userList = _userService.getUserList();
            return Ok(userList);
        }
    }
}
