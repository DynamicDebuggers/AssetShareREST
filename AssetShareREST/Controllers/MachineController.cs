using AssetShareLib;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public ActionResult<IEnumerable<Machine>> Get()
        {
            var list = _repository.GetAll();

            if (list == null || list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        // GET: api/machine/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public ActionResult<Machine> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive number.");

            var machine = _repository.GetById(id);

            if (machine == null)
                return NotFound($"No machine found with ID: {id}");

            return Ok(machine);
        }

        // POST: api/machine
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult<Machine> Post([FromBody] Machine machine)
        {
            if (machine == null)
                return BadRequest("Machine body is required.");

            try
            {
                var created = _repository.Add(machine);

                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ArgumentNullException ex) { return BadRequest(ex.Message); }
            catch (ArgumentOutOfRangeException ex) { return BadRequest(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        // PUT: api/machine/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}")]
        public ActionResult<Machine> Put(int id, [FromBody] Machine updatedMachine)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive number.");

            if (updatedMachine == null)
                return BadRequest("Machine body is required.");

            var existing = _repository.GetById(id);
            if (existing == null)
                return NotFound($"No machine found with ID: {id}");

            // NOTE: dit repo.Update overskriver felter direkte (ingen partial update)
            // Hvis du vil understøtte partial update, kan vi lave merge her.
            var result = _repository.Update(id, updatedMachine);

            if (result == null)
                return NotFound($"No machine found with ID: {id}");

            return Ok(result);
        }

        // DELETE: api/machine/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public ActionResult<Machine> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be a positive number.");

            var removed = _repository.Remove(id);

            if (removed == null)
                return NotFound($"No machine found with ID: {id}");

            return Ok(removed);
        }
    }
}
