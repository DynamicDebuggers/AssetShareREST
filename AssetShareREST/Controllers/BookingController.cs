using AssetShareLib;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AssetShareREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BookingController : Controller
    {
        private readonly BookingRepository _repository;
        public BookingController(BookingRepository bookingRepo)
        {
            _repository = bookingRepo;
        }
        //GET: api/<BookingController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<Booking>> Get()
        {
            var list = _repository.GetAll();
            if (list == null || !list.Any())
            {
                return NotFound();
            }
            if (list.Count == 0)
            {
                return NoContent();
            }
            return Ok(list);
        }

        //GET api/<BookingController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public ActionResult<Booking> Get(int id)
        {
            var booking = _repository.GetById(id);
            if (booking == null)
            {
                return NotFound($"No window found with ID: {id}");
            }
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }
            return Ok(booking);
        }

        //POST api/<BookingController>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public ActionResult<Booking> Post([FromBody] Booking booking)
        {
            try
            {
                Booking result = _repository.Create(booking.RentedByUserId, booking.BookedMachineId, booking.Period);
                return Created($"api/Booking/{result.Id}/", result);

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
        //DELETE api/<BookingController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public ActionResult<Booking> Delete(int id)
        {
            var booking = _repository.GetById(id);
            if (booking == null)
            {
                return NotFound($"No booking found with ID: {id}");
            }
            _repository.Delete(id);
            return Ok(booking);

        }
        //PUT api/<BookingController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id}")]
        public ActionResult<Booking> Update(int id, [FromBody] Booking updatedBooking)
        {
            try
            {
                bool conflict = _repository.GetAll().Any(b =>
                    b.Id != id &&
                    b.BookedMachineId == updatedBooking.BookedMachineId &&
                    b.Period == updatedBooking.Period
                );
                if (conflict)
                {
                    return Conflict("Maskinen er allerede booket på dette tidspunkt.");
                }

                Booking result = _repository.Update(id, updatedBooking);
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



   

        