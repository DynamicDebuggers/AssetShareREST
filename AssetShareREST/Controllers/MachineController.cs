using AssetShareLib;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AssetShareREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : Controller
    {
        private readonly MachineRepository _repository;

        public MachineController(MachineRepository repository)
        {
            _repository = repository;
        }

        // GET: api/<MachineController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<Machine>> Get()
        {
            var list = _repository.Get();
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

        // GET api/<MachineController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public ActionResult<Machine> Get(int id)
        {
            var machine = _repository.GetById(id);
            if (machine == null)
            {
                return NotFound($"No machine found with ID: {id}");
            }
            if (id <= 0)
            {
                return BadRequest("Invalid ID supplied.");
            }
            return Ok(machine);
        }

        // POST api/<MachineController>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult<Machine> Post([FromBody] Machine machine)
        {
            try
            {
                Machine result = _repository.Add(machine);
                return Created($"api/Machine/{result.Id}/", result);
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

        // PUT api/<MachineController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id}")]
        public ActionResult<Machine> Update(int id, [FromBody] Machine updatedMachine)
        {
            try
            {
                // Tjek for konflikt: fx maskiner med samme title på samme location
                bool conflict = _repository.Get()
                    .Any(m => m.Id != id &&
                              m.Title == updatedMachine.Title &&
                              m.Location == updatedMachine.Location);
                if (conflict)
                {
                    return Conflict("En maskine med samme titel på denne location eksisterer allerede.");
                }

                Machine? result = _repository.Update(id, updatedMachine);
                if (result == null)
                {
                    throw new KeyNotFoundException($"Machine with Id {id} not found.");
                }

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


        // DELETE api/<MachineController>/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public ActionResult<Machine> Delete(int id)
        {
            var machine = _repository.GetById(id);
            if (machine == null)
            {
                return NotFound($"No machine found with ID: {id}");
            }
            _repository.Remove(id);
            return Ok(machine);
        }
    }
}
