using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class HouseholdsController : ControllerBase
    {
        public HouseholdsController(IHouseholdService householdService)
        {
            _householdService = householdService;
        }

        private readonly IHouseholdService _householdService;


        [HttpGet("{householdId}")]
        public IActionResult Get(int householdId)
        {
            if (_householdService.GetHousehold(householdId) is not { } household) return NotFound();
            var result = HouseholdDto.Map(household);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = HouseholdDto.MapCollection(_householdService.GetAllHouseholds());
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateHouseholdModel model)
        {
            var newHousehold = _householdService.CreateHousehold(model);
            var result = HouseholdDto.Map(newHousehold);
            return Ok(result);
        }

        [HttpPost("{householdId}")]
        public IActionResult Update(int householdId, [FromBody] UpdateHouseholdModel model)
        {
            if (_householdService.UpdateHousehold(householdId, model) is not { } household) return NotFound();
            var result = HouseholdDto.Map(household);
            return Ok(result);
        }

        [HttpDelete("{householdId}")]
        public IActionResult Delete(int householdId)
        {
            if (!_householdService.DeleteHousehold(householdId)) return NotFound();
            return Ok();
        }
    }
}
