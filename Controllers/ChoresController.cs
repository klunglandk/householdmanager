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
    public class ChoresController : ControllerBase
    {
        public ChoresController(IChoreService choreService)
        {
            _choreService = choreService;
        }

        private readonly IChoreService _choreService;

        [HttpGet("{choreId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreDto> Get(int choreId)
        {
            if (_choreService.GetChore(choreId) is not { } chore) return NotFound();
            var result = ChoreDto.Map(chore);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ChoreDto>> GetAll(string? userId)
        {
            var result = ChoreDto.MapCollection(_choreService.GetAllChores(userId));
            return Ok(result);
        }

        [HttpPost("{choreId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreDto> Update(int choreId, [FromBody] UpdateChoreModel model)
        {
            if (_choreService.UpdateChore(choreId, model) is not { } chore) return NotFound();
            var result = ChoreDto.Map(chore);
            return Ok(result);
        }

        [HttpPost("{choreId}/update-future")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreDto> UpdateFutureChore(int choreId, [FromBody] UpdateChoreModel model)
        {
            if (_choreService.UpdateFutureChore(choreId, model) is not { } chore) return NotFound();
            var result = ChoreDto.Map(chore);
            return Ok(result);
        }

        [HttpPost("{choreId}/end")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChoreDto> End(int choreId)
        {
            if (_choreService.EndChore(choreId) is not { } chore) return NotFound();
            var result = ChoreDto.Map(chore);
            return Ok(result);
        }

        [HttpDelete("{choreId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int choreId)
        {
            if (!_choreService.DeleteChore(choreId)) return NotFound();
            return NoContent();
        }
    }
}
