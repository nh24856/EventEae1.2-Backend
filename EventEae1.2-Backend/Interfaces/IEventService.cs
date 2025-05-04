using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IEventService
    {
        Task<EventResponseDto> CreateEventAsync(EventDto eventDto, ClaimsPrincipal user);
        Task<EventResponseDto> GetEventByIdAsync(Guid id);
        Task<List<EventResponseDto>> GetEventsByCategoryAsync(string category);
        Task<List<EventResponseDto>> GetEventsByOrganizationAsync(string organizationName);
        Task<List<EventResponseDto>> GetEventsByOrganizerAsync(ClaimsPrincipal user);
        Task<List<EventResponseDto>> GetAllEventsAsync();
        Task<List<TicketTypeDto>> GetTicketTypesByEventIdAsync(Guid eventId);

        Task AddTicketTypesAsync(Guid eventId, List<TicketTypeDto> ticketTypes, ClaimsPrincipal user);
    }

}
