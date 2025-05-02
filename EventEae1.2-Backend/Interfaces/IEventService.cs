using EventEae1._2_Backend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IEventService
    {
        Task<EventResponseDto> CreateEventAsync(EventDto eventDto, string jwtToken);
        Task<EventResponseDto> GetEventByIdAsync(Guid id);
        Task<List<EventResponseDto>> GetEventsByCategoryAsync(string category);
        Task<List<EventResponseDto>> GetEventsByOrganizationAsync(string organizationName);
    }
}
