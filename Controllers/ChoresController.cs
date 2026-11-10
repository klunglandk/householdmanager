using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoresController : ControllerBase
    {
        public ChoresController(IChoreService choreService) {
            _choreService = choreService;
        }

        private readonly IChoreService _choreService;

        [HttpGet("{choreId}")]
        public IActionResult Get(int choreId)
        {
            if (_choreService.GetChore(choreId) is not { } chore) return NotFound();
            var result = ChoreDto.Map(chore);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll(int householdId, string? userId)
        {
            var result = ChoreDto.MapCollection(_choreService.GetAllChores(householdId, userId));
            return Ok(result);
        }

        [HttpPost("{choreId}")]
        public IActionResult Update(int choreId, [FromBody] UpdateChoreModel model)
        {
            if (_choreService.UpdateChore(choreId, model) is not { } chore) return NotFound();
            var result = ChoreDto.Map(chore);
            return Ok(result);
        }

        [HttpPost("{choreId}/update-future")]
        public IActionResult UpdateFutureChore(int choreId, [FromBody] UpdateChoreModel model)
        {
            if (_choreService.UpdateFutureChore(choreId, model) is not { } chore) return NotFound();
            var result = ChoreDto.Map(chore);
            return Ok(result);
        }

    }
}
