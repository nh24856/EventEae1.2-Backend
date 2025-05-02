namespace EventEae1._2_Backend.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }  // Used for existing events; ignored when creating new ones

        public string Name { get; set; }

        public string Venue { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ImagePath { get; set; }

        public string? OrganizerName { get; set; }  // Optional - used for display (EventList, EventDetail)

        public List<TicketTypeDto> TicketTypes { get; set; } = new();
        public Guid OrganizerId { get; internal set; }
    }
}
