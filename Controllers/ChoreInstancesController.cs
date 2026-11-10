using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoreInstancesController : ControllerBase
    {
        public ChoreInstancesController(IChoreInstanceService choreInstanceService)
        {
            _choreInstanceService = choreInstanceService;
        }

        private readonly IChoreInstanceService _choreInstanceService;

        [HttpGet("{choreInstanceId}")]
        public IActionResult Get(int choreInstanceId)
        {
            if (_choreInstanceService.GetChoreInstance(choreInstanceId) is not { } choreInstance) return NotFound();
            var result = ChoreInstanceDto.Map(choreInstance);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll(int householdId)
        {
            var result = ChoreInstanceDto.MapCollection(_choreInstanceService.GetAllChoreInstances(householdId));
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateChoreInstanceModel model)
        {
            if (_choreInstanceService.CreateChoreInstance(model) is not { } newChoreInstance) return NotFound();
            var result = ChoreInstanceDto.Map(newChoreInstance);
            return Ok(result);
        }

        [HttpPost("new-chore")]
        public IActionResult CreateWithNewChore([FromBody] CreateChoreInstanceWithNewChoreModel model)
        {
            var newChoreInstance = _choreInstanceService.CreateChoreInstanceWithNewChore(model);
            var result = ChoreInstanceDto.Map(newChoreInstance);
            return Ok(result);
        }

        [HttpPost("{choreInstanceId}/mark-as-done")]
        public IActionResult MarkAsDone(int choreInstanceId)
        {
            if (_choreInstanceService.MarkAsDone(choreInstanceId) is not { } choreInstance) return NotFound();
            return Ok(choreInstance.Map());
        }

        [HttpPost("{choreInstanceId}/clear-done-date")]
        public IActionResult ClearDoneDate(int choreInstanceId)
        {
            if (_choreInstanceService.ClearDoneDate(choreInstanceId) is not { } choreInstance) return NotFound();
            return Ok(choreInstance.Map());
        }

        [HttpDelete("{choreInstanceId}")]
        public IActionResult Delete(int choreInstanceId)
        {
            if (!_choreInstanceService.DeleteChoreInstance(choreInstanceId)) return NotFound();
            return Ok();
        }
    }
}
