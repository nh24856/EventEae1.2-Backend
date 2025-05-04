using System.ComponentModel.DataAnnotations;

namespace EventEae1._2_Backend.Models
{
    public class TicketType
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public int Price { get; set; }

        public Guid EventId { get; set; }

        public Event Event { get; set; }

        public int InitialStock { get; set; }

        public ICollection<TicketSale> TicketSales { get; set; }
    }
}
