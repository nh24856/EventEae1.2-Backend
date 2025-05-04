using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Repositories
{
    public class TicketSaleRepository
    {
        private readonly AppDbContext _context;

        public TicketSaleRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. Buy Ticket
        public async Task<TicketSale> BuyTicketAsync(CreateTicketSaleDto dto)
        {
            var ticketSale = new TicketSale
            {
                TicketTypeId = dto.TicketTypeId,
                BuyerId = dto.BuyerId,
                Quantity = dto.Quantity,
                SaleTime = DateTime.UtcNow
            };

            _context.TicketSales.Add(ticketSale);
            await _context.SaveChangesAsync();
            return ticketSale;
        }

        // 2. Pie Chart Data (Sales count per TicketType for an event)
        public async Task<List<PieChartDto>> GetPieChartDataAsync(Guid eventId)
        {
            return await _context.TicketSales
                .Where(ts => ts.TicketType.EventId == eventId)
                .GroupBy(ts => ts.TicketType.Name)
                .Select(g => new PieChartDto
                {
                    TicketTypeName = g.Key,
                    TotalTicketsSold = g.Sum(ts => ts.Quantity)
                })
                .ToListAsync();
        }

        // 3. Line Graph Data (Sales count by minute for an event)
        public async Task<List<LineGraphDto>> GetLineGraphDataAsync(Guid eventId)
        {
            return await _context.TicketSales
                .Where(ts => ts.TicketType.EventId == eventId)
                .GroupBy(ts => new
                {
                    ts.SaleTime.Year,
                    ts.SaleTime.Month,
                    ts.SaleTime.Day,
                    ts.SaleTime.Hour,
                    ts.SaleTime.Minute
                })
                .OrderBy(g => g.Key)
                .Select(g => new LineGraphDto
                {
                    Time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, g.Key.Minute, 0),
                    TicketsSold = g.Sum(ts => ts.Quantity)
                })
                .ToListAsync();
        }
    }
}
