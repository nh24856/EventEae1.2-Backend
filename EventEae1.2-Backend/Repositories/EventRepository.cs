using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEae1._2_Backend.Repository
{
    public class EventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateEventAsync(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == eventEntity.Id);
        }

        public async Task<List<Event>> GetEventsByCategoryAsync(string category)
        {
            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .Where(e => e.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByOrganizationAsync(string organizationName)
        {
            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .Where(e => e.Organizer != null &&
                            e.Organizer.Organization.ToLower() == organizationName.ToLower())
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByOrganizerAsync(Guid organizerId)
        {
            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .ToListAsync();
        }
    }
}
