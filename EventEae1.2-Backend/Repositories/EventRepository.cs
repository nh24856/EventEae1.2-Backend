using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Repository
{
    public class EventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Event> CreateEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
                throw new ArgumentNullException(nameof(eventEntity));

            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();

            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == eventEntity.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created event.");
        }

        public async Task<List<Event>> GetEventsByCategoryAsync(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("Category cannot be empty.", nameof(category));

            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .Where(e => e.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByOrganizationAsync(string organizationName)
        {
            if (string.IsNullOrEmpty(organizationName))
                throw new ArgumentException("Organization name cannot be empty.", nameof(organizationName));

            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .Where(e => e.Organizer != null &&
                           e.Organizer.Organization.ToLower().Contains(organizationName.ToLower()))
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByOrganizerAsync(Guid organizerId)
        {
            if (organizerId == Guid.Empty)
                throw new ArgumentException("Organizer ID cannot be empty.", nameof(organizerId));

            return await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Organizer)
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Event ID cannot be empty.", nameof(id));

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

        public async Task AddTicketTypesAsync(List<TicketType> ticketTypes)
        {
            if (ticketTypes == null || !ticketTypes.Any())
                throw new ArgumentException("At least one ticket type is required.", nameof(ticketTypes));

            if (ticketTypes.Any(t => t.EventId == Guid.Empty))
                throw new ArgumentException("All ticket types must have a valid EventId.");

            await _context.TicketTypes.AddRangeAsync(ticketTypes);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TicketType>> GetTicketTypesByEventIdAsync(Guid eventId)
        {
            if (eventId == Guid.Empty)
                throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

            return await _context.TicketTypes
                .Where(t => t.EventId == eventId)
                .ToListAsync();
        }
    }
}