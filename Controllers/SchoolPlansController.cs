using HouseholdManager.Dtos;
using HouseholdManager.Infrastructure;
using HouseholdManager.Models;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class SchoolPlansController : ControllerBase
    {
        public SchoolPlansController(ISchoolPlanService schoolPlanService)
        {
            _schoolPlanService = schoolPlanService;
        }

        private readonly ISchoolPlanService _schoolPlanService;

        [HttpGet("{schoolPlanId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SchoolPlanDto> Get(int schoolPlanId)
        {
            if (_schoolPlanService.GetSchoolPlan(schoolPlanId) is not { } schoolPlan) return NotFound();
            var result = SchoolPlanDto.Map(schoolPlan);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SchoolPlanDto>> GetAll(DateOnly? weekDate)
        {
            var result = SchoolPlanDto.MapCollection(_schoolPlanService.GetAllSchoolPlans(weekDate));
            return Ok(result);
        }

        [HttpGet("{schoolPlanId}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetFile(int schoolPlanId)
        {
            if (_schoolPlanService.GetSchoolPlan(schoolPlanId) is not { } schoolPlan) return NotFound();
            if (_schoolPlanService.OpenFile(schoolPlan) is not { } stream) return NotFound();

            Response.Headers.XContentTypeOptions = "nosniff";
            return File(stream, FileTypeDetector.GetContentType(schoolPlan.StoredFileName));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SchoolPlanDto>> Create([FromForm] CreateSchoolPlanModel model, CancellationToken cancellationToken)
        {
            await using var stream = model.File.OpenReadStream();
            var newSchoolPlan = await _schoolPlanService.CreateSchoolPlanAsync(model.WeekDate, model.File.FileName, stream, cancellationToken);
            var result = SchoolPlanDto.Map(newSchoolPlan);
            return CreatedAtAction(nameof(Get), new { schoolPlanId = newSchoolPlan.Id }, result);
        }

        [HttpDelete("{schoolPlanId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int schoolPlanId)
        {
            if (!_schoolPlanService.DeleteSchoolPlan(schoolPlanId)) return NotFound();
            return NoContent();
        }
    }
}
