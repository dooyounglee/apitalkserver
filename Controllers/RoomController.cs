using Microsoft.AspNetCore.Mvc;
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
    }
}
