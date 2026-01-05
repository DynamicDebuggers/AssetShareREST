using AssetShareLib;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/Listing
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Listing>>> Get()
        {
            var list = await _repository.GetAll();

            if (list == null)
                return NotFound();

            if (!list.Any())
                return NoContent();

            return Ok(list);
        }

        // GET: api/Listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Listing>> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            var listing = await _repository.GetById(id);

            if (listing == null)
                return NotFound($"No listing found with ID: {id}");

            return Ok(listing);
        }

        // POST: api/Listing
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Listing>> Post([FromBody] Listing listing)
        {
            if (listing == null)
                return BadRequest("Request body is required.");

            // Optional validation (kept from HEAD logic)
            if (string.IsNullOrWhiteSpace(listing.Title) ||
                string.IsNullOrWhiteSpace(listing.Description) ||
                listing.Price <= 0 ||
                listing.MachineId <= 0 ||
                listing.UserId <= 0)
            {
                return BadRequest("Title, Description, Price (>0), MachineId (>0) and UserId (>0) are required.");
            }

            var created = await _repository.Create(listing);

            return CreatedAtAction(
                nameof(Get),
                new { id = created.Id },
                created
            );
        }

        // PUT: api/Listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Listing>> Put(int id, [FromBody] Listing updatedListing)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            if (updatedListing == null)
                return BadRequest("Request body is required.");

            var result = await _repository.Update(id, updatedListing);

            if (result == null)
                return NotFound($"No listing found with ID: {id}");

            return Ok(result);
        }

        // DELETE: api/Listing/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Listing>> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            var existing = await _repository.GetById(id);

            if (existing == null)
                return NotFound($"No listing found with ID: {id}");

            await _repository.Delete(id);

            return Ok(existing);
        }
    }
}
