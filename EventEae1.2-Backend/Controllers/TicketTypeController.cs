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

       
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<TicketTypeDto>>> CreateTicketTypes([FromBody] CreateTicketTypesDto createTicketTypesDto)
        {
            if (createTicketTypesDto == null || createTicketTypesDto.TicketTypes == null || !createTicketTypesDto.TicketTypes.Any())
            {
                return BadRequest("At least one ticket type is required.");
            }

            if (string.IsNullOrEmpty(createTicketTypesDto.EventId) || !Guid.TryParse(createTicketTypesDto.EventId, out var eventId))
            {
                return BadRequest("Invalid event ID format.");
            }

            try
            {
                await _eventService.AddTicketTypesAsync(eventId, createTicketTypesDto.TicketTypes, User);
                var ticketTypes = await _eventService.GetTicketTypesByEventIdAsync(eventId);
                return CreatedAtAction(
                    nameof(GetTicketTypesByEventId),
                    new { eventId = createTicketTypesDto.EventId },
                    ticketTypes
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating ticket types: {ex.Message}");
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<List<TicketTypeDto>>> GetTicketTypesByEventId(string eventId)
        {
            if (string.IsNullOrEmpty(eventId) || !Guid.TryParse(eventId, out var parsedEventId))
            {
                return BadRequest("Invalid event ID format.");
            }

            try
            {
                var ticketTypes = await _eventService.GetTicketTypesByEventIdAsync(parsedEventId);
                return Ok(ticketTypes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving ticket types: {ex.Message}");
            }
        }
    }
}