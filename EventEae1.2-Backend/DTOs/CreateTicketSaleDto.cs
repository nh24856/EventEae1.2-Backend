namespace EventEae1._2_Backend.DTOs
{
    public class CreateTicketSaleDto
    {
        public Guid TicketTypeId { get; set; }
        public Guid BuyerId { get; set; }
        public int Quantity { get; set; }
    }

    public class PieChartDto
    {
        public string TicketTypeName { get; set; }
        public int TotalTicketsSold { get; set; }
    }

    public class LineGraphDto
    {
        public DateTime Time { get; set; }
        public int TicketsSold { get; set; }
    }
}
