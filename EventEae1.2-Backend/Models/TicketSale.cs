namespace EventEae1._2_Backend.Models
{
    public class TicketSale
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TicketTypeId { get; set; }
        public TicketType TicketType { get; set; }

        public Guid BuyerId { get; set; }
        public User Buyer { get; set; }

        public int Quantity { get; set; }

        public DateTime SaleTime { get; set; } = DateTime.UtcNow;

        public Guid EventId { get; set; }  // Add EventId to associate sales with events
        public Event Event { get; set; }
    }
}
