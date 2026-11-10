using HouseholdManager.Dtos;
using HouseholdManager.Infrastructure;
using HouseholdManager.Models;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolPlansController : ControllerBase
    {
        public SchoolPlansController(ISchoolPlanService schoolPlanService)
        {
            _schoolPlanService = schoolPlanService;
        }

        private readonly ISchoolPlanService _schoolPlanService;

        [HttpGet("{schoolPlanId}")]
        public IActionResult Get(int schoolPlanId)
        {
            if (_schoolPlanService.GetSchoolPlan(schoolPlanId) is not { } schoolPlan) return NotFound();
            var result = SchoolPlanDto.Map(schoolPlan);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll(int householdId, DateOnly? weekDate)
        {
            var result = SchoolPlanDto.MapCollection(_schoolPlanService.GetAllSchoolPlans(householdId, weekDate));
            return Ok(result);
        }

        [HttpGet("{schoolPlanId}/file")]
        public IActionResult GetFile(int schoolPlanId)
        {
            if (_schoolPlanService.GetSchoolPlan(schoolPlanId) is not { } schoolPlan) return NotFound();
            if (_schoolPlanService.OpenFile(schoolPlan) is not { } stream) return NotFound();

            Response.Headers.XContentTypeOptions = "nosniff";
            return File(stream, FileTypeDetector.GetContentType(schoolPlan.StoredFileName));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateSchoolPlanModel model, CancellationToken cancellationToken)
        {
            await using var stream = model.File.OpenReadStream();
            var newSchoolPlan = await _schoolPlanService.CreateSchoolPlanAsync(model.HouseholdId, model.WeekDate, model.File.FileName, stream, cancellationToken);
            if (newSchoolPlan is null) return NotFound();

            var result = SchoolPlanDto.Map(newSchoolPlan);
            return Ok(result);
        }

        [HttpDelete("{schoolPlanId}")]
        public IActionResult Delete(int schoolPlanId)
        {
            if (!_schoolPlanService.DeleteSchoolPlan(schoolPlanId)) return NotFound();
            return Ok();
        }
    }
}
