using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Models;

namespace EventEae1._2_Backend.Interfaces
{
    public interface ITicketSaleService
    {
        Task<TicketSale> BuyTicketAsync(CreateTicketSaleDto dto);
        Task<List<PieChartDto>> GetPieChartDataAsync(Guid eventId);
        Task<List<LineGraphDto>> GetLineGraphDataAsync(Guid eventId);
        Task<int> GetTotalStockByEventIdAsync(Guid eventId);
    }
}
