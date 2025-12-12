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
    public class ListingController : ControllerBase
    {
        private readonly ListingRepository _repository;

        public ListingController(ListingRepository listingRepo)
        {
            _repository = listingRepo;
        }

        // GET: api/listing
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Listing>>> Get()
        {
            var list = await _repository.GetAll();

            if (list == null || list.Count == 0)
            {
                return NoContent();
            }

            return Ok(list);
        }

        // GET: api/listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Listing>> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            var listing = await _repository.GetById(id);
            if (listing == null)
            {
                return NotFound($"No listing found with ID: {id}");
            }

            return Ok(listing);
        }

        // POST: api/listing
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Listing>> Post([FromBody] Listing listing)
        {
            try
            {
                // Hvis du har ValidateAll() på Listing, kan du evt. kalde den her
                // listing.ValidateAll();

                var created = await _repository.Create(listing);

                return CreatedAtAction(
                    nameof(Get),
                    new { id = created.Id },
                    created
                );
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Listing>> Put(int id, [FromBody] Listing updatedListing)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            try
            {
                var result = await _repository.Update(id, updatedListing);
                if (result == null)
                {
                    return NotFound($"No listing found with ID: {id}");
                }

                return Ok(result);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Listing>> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

            var existing = await _repository.GetById(id);
            if (existing == null)
            {
                return NotFound($"No listing found with ID: {id}");
            }

            await _repository.Delete(id);
            return Ok(existing);
        }
    }
}
