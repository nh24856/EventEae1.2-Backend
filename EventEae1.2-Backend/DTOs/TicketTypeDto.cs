using System.Text.Json.Serialization;

namespace EventEae1._2_Backend.DTOs
{
    public class TicketTypeDto
    {
        public string Name { get; set; }
       
        public int Price { get; set; }

        public int InitialStock { get; set; }

       
    }
}
