using HouseholdManager.Dtos;
using HouseholdManager.Models;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class HouseholdsController : ControllerBase
    {
        public HouseholdsController(IHouseholdService householdService)
        {
            _householdService = householdService;
        }

        private readonly IHouseholdService _householdService;

        [HttpGet("{householdId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<HouseholdDto> Get(int householdId)
        {
            if (_householdService.GetHousehold(householdId) is not { } household) return NotFound();
            var result = HouseholdDto.Map(household);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<HouseholdDto>> GetAll()
        {
            var result = HouseholdDto.MapCollection(_householdService.GetAllHouseholds());
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<HouseholdDto> Create([FromBody] CreateHouseholdModel model)
        {
            var newHousehold = _householdService.CreateHousehold(model);
            var result = HouseholdDto.Map(newHousehold);
            return CreatedAtAction(nameof(Get), new { householdId = newHousehold.Id }, result);
        }

        [HttpPost("{householdId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<HouseholdDto> Update(int householdId, [FromBody] UpdateHouseholdModel model)
        {
            if (_householdService.UpdateHousehold(householdId, model) is not { } household) return NotFound();
            var result = HouseholdDto.Map(household);
            return Ok(result);
        }

        [HttpDelete("{householdId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int householdId)
        {
            if (!_householdService.DeleteHousehold(householdId)) return NotFound();
            return NoContent();
        }
    }
}
