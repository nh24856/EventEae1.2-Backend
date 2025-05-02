namespace EventEae1._2_Backend.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
