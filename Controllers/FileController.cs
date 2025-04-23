using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public FileController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string fileName, [FromForm] string fileType)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized("Login required.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedTypes.Contains(ext))
                return BadRequest("Only PDF and image files are allowed.");

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = Guid.NewGuid().ToString() + ext;
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var dbFile = new MedicalFile
            {
                FileName = fileName,
                FileType = fileType,
                FilePath = $"/uploads/{uniqueName}",
                UserId = userId.Value
            };

            _context.MedicalFiles.Add(dbFile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded", fileId = dbFile.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized("Login required.");

            var files = await _context.MedicalFiles
    .Where(f => f.UserId == userId)
    .Select(f => new
    {
        f.Id,
        f.FileName,
        f.FileType,
        FileUrl = $"{Request.Scheme}://{Request.Host}{f.FilePath}"
    })
    .ToListAsync();

            return Ok(files);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized("Login required.");

            var file = await _context.MedicalFiles.FindAsync(id);
            if (file == null || file.UserId != userId)
                return NotFound("File not found.");

            var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", file.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            _context.MedicalFiles.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { message = "File deleted" });
        }
    }
}
