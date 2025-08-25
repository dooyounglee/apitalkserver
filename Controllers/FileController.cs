using Microsoft.AspNetCore.Mvc;
using rest1.Services;
using System.IO;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/v1/file")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("image")]
        public async Task<IActionResult> getImage([FromQuery] int fileNo)
        {
            var file = _fileService.getFile(fileNo);
            var imagePath = Path.Combine(file.FilePath, file.FileName);

            if (!System.IO.File.Exists(imagePath))
                return NotFound();

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpeg"); // 또는 image/png
        }

        [HttpGet("")]
        public async Task<IActionResult> getFile([FromQuery] int fileNo)
        {
            var file = _fileService.getFile(fileNo);
            var filePath = Path.Combine(file.FilePath, file.FileName);
            if (file == null || !System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = file.OriginName,
                Inline = false
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(fileBytes, contentType);
        }
    }
}
