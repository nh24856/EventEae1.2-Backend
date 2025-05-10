using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEae1._2_Backend.Data;
using System.IO;
using EventEae1._2_Backend.DTOs;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _context;

    public EventController(IWebHostEnvironment environment, AppDbContext context)
    {
        _environment = environment;
        _context = context;
    }

    [HttpPost("upload-image/{eventId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromRoute] Guid eventId ,[FromForm] UploadEventImageDto form)
    {
        if (Request.Form.Files.Count == 0)
            return BadRequest("No image uploaded.");

        var file = Request.Form.Files[0];
        if (file.Length == 0)
            return BadRequest("Uploaded image is empty.");

        try
        {
            string folderPath = Path.Combine(_environment.WebRootPath, "uploads", "event", eventId.ToString());
            Directory.CreateDirectory(folderPath);

            string imagePath = Path.Combine(folderPath, "image.png");

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update image path in DB
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null)
                return NotFound("Event not found.");

            ev.ImagePath = $"uploads/event/{eventId}/image.png";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                imagePath = ev.ImagePath,
                message = "Image uploaded successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Upload failed: {ex.Message}" });
        }
    }
    [HttpGet("get-image/{eventId}")]
    public IActionResult GetImage([FromRoute] Guid eventId)
    {
        string imagePath = Path.Combine(_environment.WebRootPath, "uploads", "event", eventId.ToString(), "image.png");

        if (!System.IO.File.Exists(imagePath))
            return NotFound("Image not found.");

        var imageBytes = System.IO.File.ReadAllBytes(imagePath);
        return File(imageBytes, "image/png"); 
    }
}
