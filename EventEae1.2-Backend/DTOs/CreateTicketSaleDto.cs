namespace EventEae1._2_Backend.DTOs
{
    public class CreateTicketSaleDto
    {
        public Guid TicketTypeId { get; set; }
        public Guid BuyerId { get; set; }

        public Guid EventId { get; set; }
        public int Quantity { get; set; }
    }

    public class PieChartDto
    {
        public string TicketTypeName { get; set; }
        public int TotalTicketsSold { get; set; }

        public int InitialStock { get; set; }

        public double Percentage { get; set; }
    }

    public class LineGraphDto
    {
        public DateTime Time { get; set; }
        public int TicketsSold { get; set; }
    }
}
