using System.Security.Cryptography;
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
    private readonly byte[] _encryptionKey; // Should be 32 bytes for AES-256
    private readonly byte[] _iv; // Initialization vector (16 bytes for AES)

    public EventController(IWebHostEnvironment environment, AppDbContext context, IConfiguration configuration)
    {
        _environment = environment;
        _context = context;

        // Get encryption key from configuration (store this securely!)
        _encryptionKey = Convert.FromBase64String(configuration["Encryption:Key"]);
        _iv = Convert.FromBase64String(configuration["Encryption:IV"]);
    }

    [HttpPost("upload-image/{eventId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromRoute] Guid eventId, [FromForm] UploadEventImageDto form)
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

            string imagePath = Path.Combine(folderPath, "image.enc");

            // Encrypt the image before saving
            await using (var inputStream = file.OpenReadStream())
            await using (var outputStream = new FileStream(imagePath, FileMode.Create))
            using (var aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.IV = _iv;

                await using (var cryptoStream = new CryptoStream(
                    outputStream,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write))
                {
                    await inputStream.CopyToAsync(cryptoStream);
                }
            }

            // Update database record
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null)
                return NotFound("Event not found.");

            ev.ImagePath = $"uploads/event/{eventId}/image.enc";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                imagePath = ev.ImagePath,
                message = "Image uploaded and encrypted successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Upload failed: {ex.Message}" });
        }
    }

    [HttpGet("get-image/{eventId}")]
    public async Task<IActionResult> GetImage([FromRoute] Guid eventId)
    {
        string encryptedImagePath = Path.Combine(_environment.WebRootPath, "uploads", "event", eventId.ToString(), "image.enc");

        if (!System.IO.File.Exists(encryptedImagePath))
            return NotFound("Image not found.");

        try
        {
            var memoryStream = new MemoryStream();

            // Decrypt the image
            await using (var inputStream = new FileStream(encryptedImagePath, FileMode.Open))
            using (var aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.IV = _iv;

                await using (var cryptoStream = new CryptoStream(
                    inputStream,
                    aes.CreateDecryptor(),
                    CryptoStreamMode.Read))
                {
                    await cryptoStream.CopyToAsync(memoryStream);
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream, "image/png");
        }
        catch (CryptographicException)
        {
            return StatusCode(500, "Failed to decrypt the image (invalid key or corrupted data).");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving image: {ex.Message}");
        }
    }
}