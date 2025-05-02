using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        
        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            if (eventDto == null)
                return BadRequest("Event data is required.");

            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Authorization token is missing or invalid.");

            var jwtToken = authHeader.Replace("Bearer ", "");

            try
            {
                var createdEvent = await _eventService.CreateEventAsync(eventDto, jwtToken);

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

        // GET: api/Event/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var events = await _eventService.GetEventsByCategoryAsync("categoryName"); // Adjust this as needed
            var eventItem = events?.Find(e => e.Id == id); // Replace this with the correct method to get an event by Id
            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }
    }
}
