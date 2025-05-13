namespace EventEae1._2_Backend.DTOs
{
    public class EventDto
    {
        public string Name { get; set; }
        public string Venue { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }

        public IFormFile? Image { get; set; }
    }

    public class CreateEventDto
    {
        public string Name { get; set; }
        public string Venue { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }
    }

}
