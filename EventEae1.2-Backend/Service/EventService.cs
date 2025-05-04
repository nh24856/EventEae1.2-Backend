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
using System.Text.Json;

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

        public async Task<EventResponseDto> CreateEventAsync(EventDto eventDto, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token claims.");
            }

            // Handle file upload
            string imagePath = null;
            if (eventDto.Image != null && eventDto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(eventDto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await eventDto.Image.CopyToAsync(stream);
                }

                imagePath = $"/Uploads/{uniqueFileName}";
            }

            // Create the event entity
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = eventDto.Name,
                Venue = eventDto.Venue,
                Date = eventDto.Date,
                Time = eventDto.Time,
                Description = eventDto.Description,
                Category = eventDto.Category,
                OrganizerId = userId,
                ImagePath = imagePath,
                TicketTypes = new List<TicketType>() // Initialize empty
            };

            var createdEvent = await _repository.CreateEventAsync(newEvent);

            if (createdEvent.Organizer == null)
            {
                createdEvent = await _repository.GetEventByIdAsync(createdEvent.Id);
            }

            return MapToResponseDto(createdEvent);
        }

        public async Task AddTicketTypesAsync(Guid eventId, List<TicketTypeDto> ticketTypes, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token claims.");
            }

            // Validate event exists and user owns it
            var eventEntity = await _repository.GetEventByIdAsync(eventId);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found.");
            }
            if (eventEntity.OrganizerId != userId)
            {
                throw new UnauthorizedAccessException("User not authorized to add ticket types for this event.");
            }

            // Map ticket types
            var ticketTypeEntities = ticketTypes.Select(t => new TicketType
            {
                Id = Guid.NewGuid(),
                Name = t.Name,
                Price = t.Price,
                EventId = eventId
            }).ToList();

            
            await _repository.AddTicketTypesAsync(ticketTypeEntities);
        }

        public async Task<List<TicketTypeDto>> GetTicketTypesByEventIdAsync(Guid eventId)
        {
            var ticketTypes = await _repository.GetTicketTypesByEventIdAsync(eventId);
            return ticketTypes.Select(t => new TicketTypeDto
            {
                Name = t.Name,
                Price = t.Price
            }).ToList();
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

        public async Task<List<EventResponseDto>> GetEventsByOrganizerAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token claims.");
            }

            var events = await _repository.GetEventsByOrganizerAsync(userId);
            return events.Select(MapToResponseDto).ToList();
        }

        public async Task<List<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _repository.GetAllEventsAsync();
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
                ImagePath = e.ImagePath,
                TicketTypes = e.TicketTypes?.Select(t => new TicketTypeDto
                {
                    Name = t.Name,
                    Price = t.Price
                }).ToList()
            };
        }

        private Guid GetUserIdFromJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token is null or empty");
                return Guid.Empty;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jsonToken == null)
                {
                    Console.WriteLine("Invalid token format");
                    return Guid.Empty;
                }

                var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Console.WriteLine("NameIdentifier claim not found");
                    Console.WriteLine("Available claims: " + string.Join(", ", jsonToken.Claims.Select(c => $"{c.Type}: {c.Value}")));
                    return Guid.Empty;
                }

                return Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation error: {ex.Message}");
                return Guid.Empty;
            }
        }
    }
}