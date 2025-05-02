using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EventEae1._2_Backend.Services
{
    public class EventService : IEventService
    {
        private readonly EventRepository _repository;
        private readonly IConfiguration _configuration;

        public EventService(EventRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<EventResponseDto> CreateEventAsync(EventDto eventDto, string jwtToken)
        {
            var userId = GetUserIdFromJwt(jwtToken);
            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid or expired JWT token");

            // 1️⃣ Handle file upload
            string imagePath = null;
            if (eventDto.Image != null && eventDto.Image.Length > 0)
            {
                // Create the uploads folder if it doesn't exist
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(eventDto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await eventDto.Image.CopyToAsync(stream);
                }

                // Set relative path to serve it via your API if needed
                imagePath = $"/uploads/{uniqueFileName}";
            }

            // 2️⃣ Create the event entity
            var newEvent = new Event
            {
                Name = eventDto.Name,
                Venue = eventDto.Venue,
                Date = eventDto.Date,
                Time = eventDto.Time,
                Description = eventDto.Description,
                Category = eventDto.Category,
                OrganizerId = userId,
                ImagePath = imagePath,  // ✅ Save the image path
                TicketTypes = eventDto.TicketTypes?.Select(t => new TicketType
                {
                    Name = t.Name,
                    Price = t.Price
                }).ToList()
            };

            var createdEvent = await _repository.CreateEventAsync(newEvent);

            // ✅ Optionally: Fetch Organizer if null (ensure your repository includes it)
            if (createdEvent.Organizer == null)
            {
                createdEvent = await _repository.GetEventByIdAsync(createdEvent.Id);
            }

            return MapToResponseDto(createdEvent);
        }


        public async Task<EventResponseDto> GetEventByIdAsync(Guid id)
        {
            var e = await _repository.GetEventByIdAsync(id);
            if (e == null)
                return null;

            return MapToResponseDto(e);
        }

        public async Task<List<EventResponseDto>> GetEventsByCategoryAsync(string category)
        {
            var events = await _repository.GetEventsByCategoryAsync(category);
            return events.Select(MapToResponseDto).ToList();
        }

        public async Task<List<EventResponseDto>> GetEventsByOrganizationAsync(string organizationName)
        {
            var events = await _repository.GetEventsByOrganizationAsync(organizationName);
            return events.Select(MapToResponseDto).ToList();
        }

        private EventResponseDto MapToResponseDto(Event e)
        {
            return new EventResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                Venue = e.Venue,
                Date = e.Date,
                Time = e.Time,
                Description = e.Description,
                Category = e.Category,
                OrganizerId = e.OrganizerId,
                OrganizerName = e.Organizer != null ? $"{e.Organizer.FirstName} {e.Organizer.LastName}" : null,
                ImagePath = e.ImagePath,  // ✅ Add this if not already there
                TicketTypes = e.TicketTypes?.Select(t => new TicketTypeDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Price = t.Price
                }).ToList()
            };
        }


        private Guid GetUserIdFromJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
                return Guid.Empty;

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Guid.Empty;

            return Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
        }

        // Optional: JWT generation method (if still needed elsewhere)
        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
