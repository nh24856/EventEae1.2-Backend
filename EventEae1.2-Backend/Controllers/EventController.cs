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
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [Authorize] // 🔥 Added this to protect the route
        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromForm] EventDto eventDto)
        {
            if (eventDto == null)
                return BadRequest("Event data is required.");

            try
            {
                // Instead of extracting the token manually, get user info from User.Claims
                var createdEvent = await _eventService.CreateEventAsync(eventDto, User);

                if (createdEvent == null)
                    return StatusCode(500, "An error occurred while creating the event.");

                return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or expired JWT token.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetEventsByCategory(string category)
        {
            var events = await _eventService.GetEventsByCategoryAsync(category);
            return Ok(events);
        }

        [HttpGet("organization/{organizationName}")]
        public async Task<IActionResult> GetEventsByOrganization(string organizationName)
        {
            var events = await _eventService.GetEventsByOrganizationAsync(organizationName);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            return Ok(eventItem);
        }
    }
}
