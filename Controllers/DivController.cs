using Microsoft.AspNetCore.Mvc;
using rest1.Services;
using System.Text.Json.Serialization;
using talkLib.Util;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/v1/div/")]
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

        [HttpPost]
        public async Task<IActionResult> postInsertDiv([FromBody] DivRequestDto dto)
        {
            _divService.InsertDiv(dto.DivNm);

            var chatList = _divService.getDivList();
            return Ok(chatList);
        }

        [HttpPut]
        public async Task<IActionResult> putEditDiv([FromBody] DivRequestDto dto)
        {
            var result = _divService.EditDiv(dto.DivNo, dto.DivNm);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> deleteDeleteDiv([FromBody] DivRequestDto dto)
        {
            _divService.DeleteDiv(dto.DivNo);
            return Ok();
        }
    }

    public class DivRequestDto
    {
        [JsonPropertyName("divNo")]
        public int DivNo { get; set; }
        [JsonPropertyName("divNm")]
        public string? DivNm { get; set; }
    }
}
