using Microsoft.AspNetCore.Mvc;
using rest1.Services;
using talkLib.Util;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/v1/div")]
    public class DivController : Controller
    {
        private readonly IDivService _divService;

        public DivController(IDivService divService)
        {
            _divService = divService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> getDivList()
        {
            var chatList = _divService.getDivList();
            return Ok(chatList);
        }

        [HttpPost("")]
        public async Task<IActionResult> postInsertDiv([FromBody] DivRequestDto dto)
        {
            var result = _divService.InsertDiv(dto.divNm);
            return Ok(result);
        }

        [HttpPut("")]
        public async Task<IActionResult> putEditDiv([FromBody] DivRequestDto dto)
        {
            var result = _divService.EditDiv(dto.divNo, dto.divNm);
            return Ok(result);
        }
    }

    public class DivRequestDto
    {
        public int divNo { get; set; }
        public string divNm { get; set; }
    }
}
