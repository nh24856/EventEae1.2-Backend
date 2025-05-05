using Microsoft.AspNetCore.Mvc;
using EventEae1._2_Backend.Services;
using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.Interfaces;

namespace EventEae1._2_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketSaleController : ControllerBase
    {
        private readonly ITicketSaleService _ticketSaleService;

        public TicketSaleController(ITicketSaleService ticketSaleService)
        {
            _ticketSaleService = ticketSaleService;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyTicket([FromBody] CreateTicketSaleDto dto)
        {
            try
            {
                var ticketSale = await _ticketSaleService.BuyTicketAsync(dto);
                return Ok(ticketSale);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("piechart/{eventId}")]
        public async Task<IActionResult> GetPieChartData(Guid eventId)
        {
            var data = await _ticketSaleService.GetPieChartDataAsync(eventId);
            return Ok(data);
        }

        [HttpGet("linegraph/{eventId}")]
        public async Task<IActionResult> GetLineGraphData(Guid eventId)
        {
            var data = await _ticketSaleService.GetLineGraphDataAsync(eventId);
            return Ok(data);
        }
        [HttpGet("total-stock/{eventId}")]
        public async Task<IActionResult> GetTotalStock(Guid eventId)
        {
            var totalStock = await _ticketSaleService.GetTotalStockByEventIdAsync(eventId);
            return Ok(totalStock);
        }
    }
}
