namespace EventEae1._2_Backend.DTOs
{
    public class EventResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Venue { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public List<TicketTypeDto> TicketTypes { get; set; }
    }
}