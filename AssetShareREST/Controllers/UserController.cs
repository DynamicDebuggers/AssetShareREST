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
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repository;

        public UserController(UserRepository userRepo)
        {
            _repository = userRepo;
        }

        // GET: api/user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _repository.GetAllAsync();

            if (users == null || users.Count == 0)
            {
                return NoContent();
            }

            return Ok(users);
        }

        // GET api/user/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be a positive number");
            }

            var user = await _repository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST api/user
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User body required");
            }

            // tjek for email-konflikt
            var existingUsers = await _repository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(user.Email) &&
                existingUsers.Any(u => u.Email == user.Email))
            {
                return Conflict("This email is already in use");
            }

            try
            {
                var created = await _repository.AddAsync(user);

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

        // PUT api/user/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] User updatedUser)
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

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            // email-konflikt: andre brugere med samme email
            var allUsers = await _repository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(updatedUser.Email) &&
                allUsers.Any(u => u.Id != id && u.Email == updatedUser.Email))
            {
                return Conflict("Another user with this email already exists.");
            }

            try
            {
                updatedUser.Id = id;
                var result = await _repository.UpdateAsync(id, updatedUser);

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

        // DELETE api/user/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be a positive number.");
            }

            var deleted = await _repository.DeleteAsync(id);

            if (deleted == null)
            {
                return NotFound();
            }

            return Ok(deleted);
        }
    }
}
