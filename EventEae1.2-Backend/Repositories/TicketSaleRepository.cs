using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            // Validate TicketType existence
            var ticketTypeExists = await _context.TicketTypes.AnyAsync(tt => tt.Id == dto.TicketTypeId);
            if (!ticketTypeExists)
                throw new Exception("Invalid TicketTypeId: Ticket type not found.");

            // Validate Buyer (User) existence
            var buyerExists = await _context.Users.AnyAsync(u => u.Id == dto.BuyerId);
            if (!buyerExists)
                throw new Exception("Invalid BuyerId: Buyer not found.");

            var ticketSale = new TicketSale
            {
                TicketTypeId = dto.TicketTypeId,
                BuyerId = dto.BuyerId,
                EventId = dto.EventId,
                Quantity = dto.Quantity,
                SaleTime = DateTime.UtcNow
            };

            try
            {
                _context.TicketSales.Add(ticketSale);
                await _context.SaveChangesAsync();
                return ticketSale;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save ticket sale: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        // 2. Pie Chart Data (Sales count per TicketType for an event)
        public async Task<List<PieChartDto>> GetPieChartDataAsync(Guid eventId)
        {
            var ticketTypes = await _context.TicketTypes
                .Where(tt => tt.EventId == eventId)
                .Select(tt => new
                {
                    tt.Name,
                    tt.InitialStock,
                    TicketsSold = _context.TicketSales
                        .Where(ts => ts.TicketTypeId == tt.Id)
                        .Sum(ts => (int?)ts.Quantity) ?? 0
                })
                .ToListAsync();

            var totalInitialStock = ticketTypes.Sum(tt => tt.InitialStock);

            return ticketTypes.Select(tt => new PieChartDto
            {
                TicketTypeName = tt.Name,
                TotalTicketsSold = tt.TicketsSold,
                InitialStock = tt.InitialStock,
                Percentage = totalInitialStock == 0 ? 0 : (double)tt.TicketsSold / totalInitialStock * 100
            }).ToList();
        }

        // 3. Line Graph Data (Sales count by minute for an event)
        public async Task<List<LineGraphDto>> GetLineGraphDataAsync(Guid eventId)
        {
            // Fetch all relevant sales with TicketType included
            var sales = await _context.TicketSales
                .Where(ts => ts.TicketType.EventId == eventId)
                .ToListAsync();

            // Group in memory by exact minute
            var result = sales
                .GroupBy(ts => new DateTime(
                    ts.SaleTime.Year,
                    ts.SaleTime.Month,
                    ts.SaleTime.Day,
                    ts.SaleTime.Hour,
                    ts.SaleTime.Minute,
                    0)) // round down to minute
                .OrderBy(g => g.Key)
                .Select(g => new LineGraphDto
                {
                    Time = g.Key,
                    TicketsSold = g.Sum(ts => ts.Quantity)
                })
                .ToList();

            return result;
        }
        public async Task<int> GetTotalStockByEventIdAsync(Guid eventId)
        {
            return await _context.TicketTypes
                .Where(tt => tt.EventId == eventId)
                .SumAsync(tt => tt.InitialStock);
        }

    }
}
