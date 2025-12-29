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
    public class MachineController : ControllerBase
    {
        private readonly MachineRepository _repository;

        public MachineController(MachineRepository machineRepo)
        {
            _repository = machineRepo;
        }

        // GET: api/machine
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machine>>> Get()
        {
            var machines = await _repository.Get();

            if (machines == null)
                return NotFound();

            if (machines.Count == 0)
                return NoContent();

            return Ok(machines);
        }

        // GET: api/machine/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Machine>> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive number.");

            var machine = await _repository.GetById(id);

            if (machine == null)
                return NotFound($"No machine found with ID: {id}");

            return Ok(machine);
        }

        // POST: api/machine
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Machine>> Post([FromBody] Machine machine)
        {
            if (machine == null)
                return BadRequest("Machine body is required.");

            try
            {
                if (string.IsNullOrWhiteSpace(machine.Title))
                    return BadRequest("Title is required.");

                if (machine.Price <= 0)
                    return BadRequest("Price must be greater than zero.");

                var created = await _repository.Add(machine);

                return CreatedAtAction(
                    nameof(Get),
                    new { id = created.Id },
                    created
                );
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/machine/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Machine>> Put(int id, [FromBody] Machine updatedMachine)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive number.");

            if (updatedMachine == null)
                return BadRequest("Machine body is required.");

            // Ensure body Id matches route Id (if provided)
            if (updatedMachine.Id != 0 && updatedMachine.Id != id)
                return BadRequest("Body Id must match route Id.");

            var existing = await _repository.GetById(id);
            if (existing == null)
                return NotFound($"No machine found with ID: {id}");

            try
            {
                // Optional conflict check (from HEAD idea): same Title + Location for another machine
                // Only run if your domain actually uses Location meaningfully.
                var allMachines = await _repository.Get();
                if (allMachines != null)
                {
                    bool conflict = allMachines.Any(m =>
                        m.Id != id &&
                        string.Equals(m.Title, updatedMachine.Title, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(m.Location, updatedMachine.Location, StringComparison.OrdinalIgnoreCase));

                    if (conflict)
                        return Conflict("A machine with the same title at this location already exists.");
                }

                updatedMachine.Id = id;

                var result = await _repository.Update(id, updatedMachine);

                if (result == null)
                    return NotFound($"No machine found with ID: {id}");

                return Ok(result);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/machine/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Machine>> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive number.");

            var removed = await _repository.Remove(id);

            if (removed == null)
                return NotFound($"No machine found with ID: {id}");

            return Ok(removed);
        }
    }
}
