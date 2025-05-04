namespace EventEae1._2_Backend.DTOs
{
   
        public class CreateTicketTypesDto
        {
            public string EventId { get; set; }
            public List<TicketTypeDto> TicketTypes { get; set; }
        }
    
}
