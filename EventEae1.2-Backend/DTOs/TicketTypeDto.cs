using System.Text.Json.Serialization;

namespace EventEae1._2_Backend.DTOs
{
    public class TicketTypeDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("price")]
        public int Price { get; set; }
        public Guid Id { get; internal set; }
    }
}
