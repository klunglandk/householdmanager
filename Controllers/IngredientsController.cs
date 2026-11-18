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
    public class IngredientsController : ControllerBase
    {
        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        private readonly IIngredientService _ingredientService;

        [HttpGet("{ingredientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IngredientDto> Get(int ingredientId)
        {
            if (_ingredientService.GetIngredient(ingredientId) is not { } ingredient) return NotFound();
            var result = IngredientDto.Map(ingredient);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<IngredientDto>> GetAll(int recipeId)
        {
            var result = IngredientDto.MapCollection(_ingredientService.GetAllIngredients(recipeId));
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IngredientDto> Create([FromBody] CreateIngredientModel model)
        {
            if (_ingredientService.CreateIngredient(model) is not { } newIngredient) return NotFound();
            var result = IngredientDto.Map(newIngredient);
            return CreatedAtAction(nameof(Get), new { ingredientId = newIngredient.Id }, result);
        }

        [HttpPost("{ingredientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IngredientDto> Update(int ingredientId, [FromBody] UpdateIngredientModel model)
        {
            if (_ingredientService.UpdateIngredient(ingredientId, model) is not { } ingredient) return NotFound();
            var result = IngredientDto.Map(ingredient);
            return Ok(result);
        }

        [HttpDelete("{ingredientId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int ingredientId)
        {
            if (!_ingredientService.DeleteIngredient(ingredientId)) return NotFound();
            return NoContent();
        }
    }
}
