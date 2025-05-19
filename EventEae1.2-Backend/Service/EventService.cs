using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.Repository;
using EventEae1._2_Backend.Services;
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
        private readonly IAuditLogService _auditLogService;

        public EventService(EventRepository repository, IConfiguration configuration, IAuditLogService auditLogService)
        {
            _repository = repository;
            _configuration = configuration;
            _auditLogService = auditLogService;
        }

        public async Task<EventResponseDto> CreateEventAsync(CreateEventDto eventDto, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var userEmailClaim = user.FindFirst(JwtRegisteredClaimNames.Email);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token claims.");
            }

            var userEmail = userEmailClaim?.Value ?? "unknown";

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
                OrganizerId = userId
            };

            var createdEvent = await _repository.CreateEventAsync(newEvent);

            // Audit log for event creation
            await _auditLogService.LogAsync(userId.ToString(), userEmail, "EventCreated",
                "Event", createdEvent.Id.ToString(), null, new
                {
                    Name = createdEvent.Name,
                    Venue = createdEvent.Venue,
                    Date = createdEvent.Date,
                    Category = createdEvent.Category
                });

            if (createdEvent.Organizer == null)
            {
                createdEvent = await _repository.GetEventByIdAsync(createdEvent.Id);
            }

            return MapToResponseDto(createdEvent);
        }

        public async Task AddTicketTypesAsync(string eventId, List<TicketTypeDto> ticketTypes, ClaimsPrincipal user)
        {
            // Convert string to Guid
            if (!Guid.TryParse(eventId, out var parsedEventId) || parsedEventId == Guid.Empty)
            {
                throw new ArgumentException("Invalid event ID.");
            }

            var eventEntity = await _repository.GetEventByIdAsync(parsedEventId);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var userEmailClaim = user.FindFirst(JwtRegisteredClaimNames.Email);
            var userId = userIdClaim?.Value ?? "unknown";
            var userEmail = userEmailClaim?.Value ?? "unknown";

            var ticketTypeEntities = ticketTypes.Select(t => new TicketType
            {
                Id = Guid.NewGuid(),
                Name = t.Name,
                Price = t.Price,
                InitialStock = t.InitialStock,
                EventId = parsedEventId
            }).ToList();

            await _repository.AddTicketTypesAsync(ticketTypeEntities);

            // Audit log for ticket types addition
            await _auditLogService.LogAsync(userId, userEmail, "TicketTypesAdded",
                "Event", parsedEventId.ToString(), null, new
                {
                    EventName = eventEntity.Name,
                    TicketTypes = ticketTypes.Select(t => new { t.Name, t.Price, t.InitialStock })
                });
        }

        public async Task<List<TicketTypeDto>> GetTicketTypesByEventIdAsync(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                throw new ArgumentException("Invalid event ID.");
            }

            var ticketTypes = await _repository.GetTicketTypesByEventIdAsync(eventId);
            return ticketTypes.Select(t => new TicketTypeDto
            {
                Name = t.Name,
                Price = t.Price,
                InitialStock = t.InitialStock
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
                OrganizerName = e.Organizer != null ? $"{e.Organizer.FirstName} {e.Organizer.LastName}" : null
            };
        }
    }
}