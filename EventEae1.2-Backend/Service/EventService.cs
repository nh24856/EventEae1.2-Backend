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

        public async Task<EventDto> CreateEventAsync(EventDto eventDto, string jwtToken)
        {
            // Get User information from JWT token
            var userId = GetUserIdFromJwt(jwtToken);
            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("Invalid or expired JWT token");

            var newEvent = new Event
            {
                Name = eventDto.Name,
                Venue = eventDto.Venue,
                Date = eventDto.Date,
                Time = eventDto.Time,
                Description = eventDto.Description,
                Category = eventDto.Category,
                OrganizerId = userId, // User ID from JWT
                TicketTypes = eventDto.TicketTypes?.Select(t => new TicketType
                {
                    Name = t.Name,
                    Price = t.Price
                }).ToList()
            };

            var createdEvent = await _repository.CreateEventAsync(newEvent);
            return MapToDto(createdEvent);
        }

        public async Task<List<EventDto>> GetEventsByCategoryAsync(string category)
        {
            var events = await _repository.GetEventsByCategoryAsync(category);
            return events.Select(MapToDto).ToList();
        }

        public async Task<List<EventDto>> GetEventsByOrganizationAsync(string organizationName)
        {
            var events = await _repository.GetEventsByOrganizationAsync(organizationName);
            return events.Select(MapToDto).ToList();
        }

        private EventDto MapToDto(Event e)
        {
            return new EventDto
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

        // Optional: Method to generate JWT token for users (e.g., for login endpoint)
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
