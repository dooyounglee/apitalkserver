using Microsoft.AspNetCore.Mvc;
using OTILib.Util;
using rest1.Attibutes;
using rest1.Models;
using rest1.Services;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/v1/room/")]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Log]
        [HttpGet("list")]
        public async Task<IActionResult> getRoomList([FromQuery] int usrNo)
        {
            var roomList = _roomService.getRoomList(usrNo);
            return Ok(roomList);
        }

        [HttpGet("{roomNo}")]
        public async Task<IActionResult> getRoom(int roomNo, [FromQuery] int usrNo)
        {
            var room = _roomService.getRoom(roomNo, usrNo);
            return Ok(room);
        }

        [HttpPost("create")]
        public async Task<IActionResult> createRoom([FromBody] CreateRoomDto dto)
        {
            var roomNo = _roomService.createRoom(dto.userList, dto.me);
            return Ok(roomNo);
        }

        [HttpPut("title")]
        public async Task<IActionResult> putTitle([FromBody] PutTitleDto dto)
        {
            var result = _roomService.EditTitle(dto.roomNo, dto.usrNo, dto.title);
            return Ok(result);
        }

        [HttpPost("leave")]
        public async Task<IActionResult> postLeave([FromBody] PostLeaveDto dto)
        {
            var result = _roomService.Leave(dto.roomNo, dto.usrNo, dto.msg);
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> getUsers([FromQuery] int roomNo)
        {
            var result = _roomService.RoomUserList(roomNo);
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> getCount([FromQuery] int meNo, [FromQuery] int usrNo)
        {
            var result = _roomService.CountRoomWithMe(meNo, usrNo);
            return Ok(result);
        }
    }

    public class CreateRoomDto
    {
        public List<User> userList { get; set; }
        public User me { get; set; }
    }

    public class PutTitleDto
    {
        public int roomNo { get; set; }
        public int usrNo { get; set; }
        public string title { get; set; }
    }

    public class PostLeaveDto
    {
        public int roomNo { get; set; }
        public int usrNo { get; set; }
        public string msg { get; set; }
    }
}
