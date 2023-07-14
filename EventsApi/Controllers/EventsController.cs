using EventsApi.Entities;
using EventsApi.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventsDbContext _context;

        public EventsController(EventsDbContext context)
        {
            _context = context;
        }


        // GET: api/<EventsController>
        /// <summary>
        /// Get all Events
        /// </summary>
        /// <returns>Event Collection</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();

            return Ok(devEvents);
        }

        // GET api/<EventsController>/5
        /// <summary>
        /// Get an Event
        /// </summary>
        /// <param name="id">Event Identifier</param>
        /// <returns>Event Data</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            return Ok(devEvent);
        }


        // POST api/<EventsController>
        /// <summary>
        /// Register an Event
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2023-07-27T17:59:14.141Z","endDate":"2023-07-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="devEvent">Event Data</param>
        /// <returns>Newly Created Object</returns>
        /// <response code="201">Success</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        // PUT api/<EventsController>/5
        /// <summary>
        /// Update an Event
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2023-07-27T17:59:14.141Z","endDate":"2023-07-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="id">Event Identifier</param>
        /// <param name="input">Event Data</param>
        /// <returns>Nothing.</returns>
        /// <response code="404">Not Found.</response>
        /// <response code="204">Success</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(Guid id, DevEvent input)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE api/<EventsController>/5
        // <summary>
        /// Delete an Event
        /// </summary>
        /// <param name="id">Event Identifier</param>
        /// <returns>Nothing</returns>
        /// <response code="404">Not Found</response>
        /// <response code="204">Sucess</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Register Speaker
        /// </summary>
        /// <remarks>
        /// {"name":"string","talkTitle":"string","talkDescription":"string","linkedInProfile":"string"}
        /// </remarks>
        /// <param name="id">Event Identifier</param>
        /// <param name="speaker">Speaker Data</param>
        /// <returns>Nothing.</returns>
        /// <response code="204">Sucess</response>
        /// <response code="404">Event Not Found</response>
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PostSpeaker(Guid id, DevEventSpeaker speaker)
        {
            speaker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
            {
                return NotFound();
            }

            _context.DevEventSpeakers.Add(speaker);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
