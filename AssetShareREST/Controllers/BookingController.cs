using AssetShareLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<Booking>>> Get()
        {
            var list = await _repository.GetAll();

            if (list == null || list.Count == 0)
            {
                return NoContent();
            }

            return Ok(list);
        }

        // GET api/booking/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Booking>> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            var booking = await _repository.GetById(id);
            if (booking == null)
            {
                return NotFound($"No booking found with ID: {id}");
            }

            return Ok(booking);
        }

        // POST api/booking
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<ActionResult<Booking>> Post([FromBody] Booking booking)
        {
            try
            {
                // (valgfrit) tjek for konflikt før vi opretter
                var existing = await _repository.GetAll();
                bool conflict = existing.Any(b =>
                    b.BookedMachineId == booking.BookedMachineId &&
                    b.Period == booking.Period
                );

                if (conflict)
                {
                    return Conflict("Maskinen er allerede booket på dette tidspunkt.");
                }

                var result = await _repository.Create(booking);

                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
        }

        // DELETE api/booking/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Booking>> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            var booking = await _repository.GetById(id);
            if (booking == null)
            {
                return NotFound($"No booking found with ID: {id}");
            }

            await _repository.Delete(id);
            return Ok(booking);
        }

        // PUT api/booking/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Booking>> Update(int id, [FromBody] Booking updatedBooking)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            try
            {
                // Konflikt: anden booking med samme maskine + periode
                var all = await _repository.GetAll();
                bool conflict = all.Any(b =>
                    b.Id != id &&
                    b.BookedMachineId == updatedBooking.BookedMachineId &&
                    b.Period == updatedBooking.Period
                );

                if (conflict)
                {
                    return Conflict("Maskinen er allerede booket på dette tidspunkt.");
                }

                var result = await _repository.Update(id, updatedBooking);

                // vores Mongo–repo smider KeyNotFoundException hvis den ikke findes
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
        }
    }
}
