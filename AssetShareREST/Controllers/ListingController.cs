using AssetShareLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace AssetShareREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : Controller
    {
        private readonly ListingRepository _repository;

        public ListingController(ListingRepository listingRepo)
        {
            _repository = listingRepo;
        }

        // GET: api/Listing
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<Listing>> Get()
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

        // GET api/Listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public ActionResult<Listing> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            var listing = _repository.GetById(id);
            if (listing == null)
            {
                return NotFound($"No listing found with ID: {id}");
            }

            return Ok(listing);
        }

        // POST api/Listing
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult<Listing> Post([FromBody] Listing listing)
        {
            if (listing == null)
            {
                return BadRequest("Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(listing.Title) ||
                string.IsNullOrWhiteSpace(listing.Description) ||
                listing.Price <= 0 ||
                listing.MachineId <= 0 ||
                listing.UserId <= 0)
            {
                return BadRequest("Title, Description, Price (>0), MachineId (>0) and UserId (>0) are required.");
            }

            var created = _repository.Create(listing.Title, listing.Description, listing.Price, listing.MachineId, listing.UserId);
            return Created($"api/Listing/{created.Id}/", created);
        }

        // PUT api/Listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public ActionResult<Listing> Put(int id, [FromBody] Listing updatedListing)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            if (updatedListing == null)
            {
                return BadRequest("Request body is required.");
            }

            var result = _repository.Update(id, updatedListing);
            if (result == null)
            {
                return NotFound($"No listing found with ID: {id}");
            }

            return Ok(result);
        }

        // DELETE api/Listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public ActionResult<Listing> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            var existing = _repository.GetById(id);
            if (existing == null)
            {
                return NotFound($"No listing found with ID: {id}");
            }

            _repository.Delete(id);
            return Ok(existing);
        }
    }
}
