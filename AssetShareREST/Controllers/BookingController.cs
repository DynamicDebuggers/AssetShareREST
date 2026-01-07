using AssetShareLib;
using Microsoft.AspNetCore.Mvc;

namespace AssetShareREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingRepository _repository;

        public BookingController(BookingRepository bookingRepo)
        {
            _repository = bookingRepo;
        }

        // GET: api/booking
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<Booking>> Get()
        {
            var list = _repository.GetAll();

            if (list == null || list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        // GET: api/booking/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public ActionResult<Booking> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            var booking = _repository.GetById(id);
            if (booking == null)
                return NotFound($"No booking found with ID: {id}");

            return Ok(booking);
        }

        // POST: api/booking
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public ActionResult<Booking> Post([FromBody] Booking booking)
        {
            if (booking == null)
                return BadRequest("Request body is required.");

            try
            {
                var result = _repository.Create(
                    booking.RentedByUserId,
                    booking.BookedMachineId,
                    booking.StartDate,
                    booking.EndDate
                );

                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (ArgumentOutOfRangeException e) { return BadRequest(e.Message); }
            catch (ArgumentNullException e) { return BadRequest(e.Message); }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return Conflict(e.Message); }
        }

        // PUT: api/booking/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id:int}")]
        public ActionResult<Booking> Put(int id, [FromBody] Booking updatedBooking)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            if (updatedBooking == null)
                return BadRequest("Request body is required.");

            try
            {
                var result = _repository.Update(id, updatedBooking);
                return Ok(result);
            }
            catch (KeyNotFoundException e) { return NotFound(e.Message); }
            catch (ArgumentOutOfRangeException e) { return BadRequest(e.Message); }
            catch (ArgumentNullException e) { return BadRequest(e.Message); }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return Conflict(e.Message); } // overlap/conflict
        }

        // DELETE: api/booking/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public ActionResult<Booking> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            var booking = _repository.GetById(id);
            if (booking == null)
                return NotFound($"No booking found with ID: {id}");

            _repository.Delete(id);
            return Ok(booking);
        }
    }
}
