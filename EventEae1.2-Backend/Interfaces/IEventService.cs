using EventEae1._2_Backend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IEventService
    {
        Task<EventDto> CreateEventAsync(EventDto eventDto, string jwtToken);  // Ensure this method signature matches
        Task<List<EventDto>> GetEventsByCategoryAsync(string category);
        Task<List<EventDto>> GetEventsByOrganizationAsync(string organizationName);
    }
}
