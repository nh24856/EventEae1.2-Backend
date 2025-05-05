using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Services
{
    public class TicketSaleService : ITicketSaleService
    {
        private readonly TicketSaleRepository _repository;

        public TicketSaleService(TicketSaleRepository repository)
        {
            _repository = repository;
        }

        public async Task<TicketSale> BuyTicketAsync(CreateTicketSaleDto dto)
        {
            return await _repository.BuyTicketAsync(dto);
        }

        public async Task<List<PieChartDto>> GetPieChartDataAsync(Guid eventId)
        {
            return await _repository.GetPieChartDataAsync(eventId);
        }

        public async Task<List<LineGraphDto>> GetLineGraphDataAsync(Guid eventId)
        {
            return await _repository.GetLineGraphDataAsync(eventId);
        }
        public async Task<int> GetTotalStockByEventIdAsync(Guid eventId)
        {
            return await _repository.GetTotalStockByEventIdAsync(eventId);
        }
    }
}
