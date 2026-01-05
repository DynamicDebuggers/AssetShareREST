using AssetShareLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace AssetShareREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewRepository _repository;

        public ReviewController(ReviewRepository reviewRepo)
        {
            _repository = reviewRepo;
        }

        // GET: api/Review
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> Get()
        {
            var list = await _repository.GetAllAsync();

            if (!list.Any())
                return NoContent();

            return Ok(list);
        }

        // GET: api/Review/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Review>> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            var review = await _repository.GetByIdAsync(id);

            if (review == null)
                return NotFound($"No review found with ID: {id}");

            return Ok(review);
        }

        // GET: api/Review/Listing/{listingId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("Listing/{listingId:int}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetByListingId(int listingId)
        {
            if (listingId <= 0)
                return BadRequest("Invalid listing ID supplied.");

            var list = await _repository.GetByListingIdAsync(listingId);

            if (!list.Any())
                return NoContent();

            return Ok(list);
        }

        // GET: api/Review/User/{userId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("User/{userId:int}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetByUserId(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID supplied.");

            var list = await _repository.GetByUserIdAsync(userId);

            if (!list.Any())
                return NoContent();

            return Ok(list);
        }

        // GET: api/Review/Rating/{listingId}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("Rating/{listingId:int}")]
        public async Task<ActionResult<double>> GetAverageRating(int listingId)
        {
            if (listingId <= 0)
                return BadRequest("Invalid listing ID supplied.");

            var avgRating = await _repository.GetAverageRatingByListingIdAsync(listingId);
            return Ok(avgRating);
        }

        // POST: api/Review
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Review>> Post([FromBody] Review review)
        {
            if (review == null)
                return BadRequest("Request body is required.");

            if (review.Rating < 1 || review.Rating > 5 ||
                string.IsNullOrWhiteSpace(review.Title) ||
                string.IsNullOrWhiteSpace(review.Content) ||
                review.ListingId <= 0 ||
                review.UserId <= 0)
            {
                return BadRequest("Title, Content, Rating (1-5), ListingId (>0) and UserId (>0) are required.");
            }

            var created = await _repository.AddAsync(review);

            return CreatedAtAction(
                nameof(Get),
                new { id = created.Id },
                created
            );
        }

        // PUT: api/Review/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Review>> Put(int id, [FromBody] Review updatedReview)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            if (updatedReview == null)
                return BadRequest("Request body is required.");

            var result = await _repository.UpdateAsync(id, updatedReview);

            if (result == null)
                return NotFound($"No review found with ID: {id}");

            return Ok(result);
        }

        // DELETE: api/Review/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Review>> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID supplied.");

            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                return NotFound($"No review found with ID: {id}");

            await _repository.DeleteAsync(id);

            return Ok(existing);
        }
    }
}