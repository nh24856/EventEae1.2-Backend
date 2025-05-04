using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketTypeController : ControllerBase
    {
        private readonly IEventService _eventService;

        public TicketTypeController(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

      
        [HttpPost]

        [Consumes("application/json")]
        public async Task<ActionResult> CreateTicketTypes([FromBody] CreateTicketTypesDto createTicketTypesDto)
        {
            if (createTicketTypesDto == null || createTicketTypesDto.ticketTypes == null || !createTicketTypesDto.ticketTypes.Any())
            {
                return BadRequest(new { error = "At least one ticket type is required." });
            }

            if (string.IsNullOrEmpty(createTicketTypesDto.eventId))
            {
                return BadRequest(new { error = "Event ID cannot be empty or invalid." });
            }

            foreach (var ticket in createTicketTypesDto.ticketTypes)
            {
                if (string.IsNullOrEmpty(ticket.Name))
                {
                    return BadRequest(new { error = "Ticket type name is required." });
                }
                if (ticket.Price < 0)
                {
                    return BadRequest(new { error = "Ticket price cannot be negative." });
                }
                if (ticket.InitialStock < 0)
                {
                    return BadRequest(new { error = "Initial stock cannot be negative." });
                }
            }

            try
            {
                await _eventService.AddTicketTypesAsync(createTicketTypesDto.eventId, createTicketTypesDto.ticketTypes, User);
                return Ok(new { message = "Ticket types created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while creating ticket types.", details = ex.Message });
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<List<TicketTypeDto>>> GetTicketTypesByEventId(Guid eventId)
        {
  

            try
            {
                var ticketTypes = await _eventService.GetTicketTypesByEventIdAsync(eventId);
                return Ok(ticketTypes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving ticket types.", details = ex.Message });
            }
        }
    }
}