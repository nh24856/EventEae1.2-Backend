using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace EventEae1._2_Backend.DTOs
{

    public class CreateTicketTypesDto
    {
        public string eventId { get; set; }  // Lowercase to match request
        public List<TicketTypeDto> ticketTypes { get; set; }
    }

}
