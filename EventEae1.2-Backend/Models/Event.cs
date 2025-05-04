using System.Net.Sockets;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEae1._2_Backend.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Venue { get; set; }
        public string Category { get; set; }

        public string ImagePath { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; } // Alternatively use TimeSpan

        public string Description { get; set; }

        public Guid OrganizerId { get; set; }

        // Navigation Property to the User (Organizer)
        public User Organizer { get; set; }

        public List<TicketType> TicketTypes { get; set; } = new List<TicketType>();



        public ICollection<TicketSale> TicketSales { get; set; }
    }
}