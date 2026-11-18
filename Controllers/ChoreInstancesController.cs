using HouseholdManager.Dtos;
using HouseholdManager.Models;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class ChoreInstancesController : ControllerBase
    {
        public ChoreInstancesController(IChoreInstanceService choreInstanceService)
        {
            _choreInstanceService = choreInstanceService;
        }

        private readonly IChoreInstanceService _choreInstanceService;

        [HttpGet("{choreInstanceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreInstanceDto> Get(int choreInstanceId)
        {
            if (_choreInstanceService.GetChoreInstance(choreInstanceId) is not { } choreInstance) return NotFound();
            var result = ChoreInstanceDto.Map(choreInstance);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ChoreInstanceDto>> GetAll(DateTimeOffset? from, DateTimeOffset? to, string? userId, bool onlyOpen)
        {
            var result = ChoreInstanceDto.MapCollection(_choreInstanceService.GetAllChoreInstances(from, to, userId, onlyOpen));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreInstanceDto> Create([FromBody] CreateChoreInstanceModel model)
        {
            if (_choreInstanceService.CreateChoreInstance(model) is not { } newChoreInstance) return NotFound();
            var result = ChoreInstanceDto.Map(newChoreInstance);
            return CreatedAtAction(nameof(Get), new { choreInstanceId = newChoreInstance.Id }, result);
        }

        [HttpPost("new-chore")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<ChoreInstanceDto> CreateWithNewChore([FromBody] CreateChoreInstanceWithNewChoreModel model)
        {
            var newChoreInstance = _choreInstanceService.CreateChoreInstanceWithNewChore(model);
            var result = ChoreInstanceDto.Map(newChoreInstance);
            return CreatedAtAction(nameof(Get), new { choreInstanceId = newChoreInstance.Id }, result);
        }

        [HttpPost("{choreInstanceId}/mark-as-done")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreInstanceDto> MarkAsDone(int choreInstanceId)
        {
            if (_choreInstanceService.MarkAsDone(choreInstanceId) is not { } choreInstance) return NotFound();
            return Ok(ChoreInstanceDto.Map(choreInstance));
        }

        [HttpPost("{choreInstanceId}/clear-done-date")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreInstanceDto> ClearDoneDate(int choreInstanceId)
        {
            if (_choreInstanceService.ClearDoneDate(choreInstanceId) is not { } choreInstance) return NotFound();
            return Ok(ChoreInstanceDto.Map(choreInstance));
        }

        [HttpPost("{choreInstanceId}/mark-as-skipped")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreInstanceDto> MarkAsSkipped(int choreInstanceId)
        {
            if (_choreInstanceService.MarkAsSkipped(choreInstanceId) is not { } choreInstance) return NotFound();
            return Ok(ChoreInstanceDto.Map(choreInstance));
        }

        [HttpPost("{choreInstanceId}/clear-skipped-date")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreInstanceDto> ClearSkippedDate(int choreInstanceId)
        {
            if (_choreInstanceService.ClearSkippedDate(choreInstanceId) is not { } choreInstance) return NotFound();
            return Ok(ChoreInstanceDto.Map(choreInstance));
        }

        [HttpDelete("{choreInstanceId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int choreInstanceId)
        {
            if (!_choreInstanceService.DeleteChoreInstance(choreInstanceId)) return NotFound();
            return NoContent();
        }
    }
}
