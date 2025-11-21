using AssetShareLib;
using Microsoft.AspNetCore.Mvc;

namespace AssetShareREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : Controller
    {
        private readonly UserRepository _repository;
        public UserController(UserRepository userRepo)
        {
            _repository = userRepo;
        }
        //GET: api/<UserController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            var users = _repository.GetAll();

            if (users == null || !users.Any())
            {
                return NoContent();
            }

            return Ok(users);
        }

        //GET api/<UserController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be a positive number");
            }

            var user = _repository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        //POST api/<UserController>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public ActionResult<User> Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User body required");
            }

            if (!string.IsNullOrWhiteSpace(user.Email) &&
                _repository.GetAll().Any(u => u.Email == user.Email))
            {
                return Conflict("This email is already in use");
            }

            try
            {
                var created = _repository.Add(user);

                return CreatedAtAction(
                    nameof(Get),
                    new { id = created.Id },
                    created
                    );
            }

            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
    }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
}
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //PUT api/<UserController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, [FromBody] User updatedUser)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be a positive number.");
            }

            if (updatedUser == null)
            {
                return BadRequest("User body is required.");
            }

            if (updatedUser.Id != 0 && updatedUser.Id != id)
            {
                return BadRequest("Body Id must match route Id.");
            }

            var existing = _repository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(updatedUser.Email) &&
                _repository.GetAll().Any(u => u.Id != id && u.Email == updatedUser.Email))
            {
                return Conflict("Another user with this email already exists.");
            }

            try
            {
                updatedUser.Id = id;
                var result = _repository.Update(id, updatedUser);

                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE api/<UserController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public ActionResult<User> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be a positive number.");
            }

            var deleted = _repository.Delete(id);

            if (deleted == null)
            {
                return NotFound();
            }

            return Ok(deleted);
        }
    }
}





