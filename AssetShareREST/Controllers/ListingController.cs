using AssetShareLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Linq;
=======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
>>>>>>> Test-branch

namespace AssetShareREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
<<<<<<< HEAD
    public class ListingController : Controller
=======
    public class ListingController : ControllerBase
>>>>>>> Test-branch
    {
        private readonly ListingRepository _repository;

        public ListingController(ListingRepository listingRepo)
        {
            _repository = listingRepo;
        }

<<<<<<< HEAD
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
=======
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
>>>>>>> Test-branch
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

<<<<<<< HEAD
            var listing = _repository.GetById(id);
=======
            var listing = await _repository.GetById(id);
>>>>>>> Test-branch
            if (listing == null)
            {
                return NotFound($"No listing found with ID: {id}");
            }

            return Ok(listing);
        }

<<<<<<< HEAD
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
=======
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
>>>>>>> Test-branch
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

<<<<<<< HEAD
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
=======
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
>>>>>>> Test-branch
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }

<<<<<<< HEAD
            var existing = _repository.GetById(id);
=======
            var existing = await _repository.GetById(id);
>>>>>>> Test-branch
            if (existing == null)
            {
                return NotFound($"No listing found with ID: {id}");
            }

<<<<<<< HEAD
            _repository.Delete(id);
=======
            await _repository.Delete(id);
>>>>>>> Test-branch
            return Ok(existing);
        }
    }
}
