using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        private readonly IIngredientService _ingredientService;

        [HttpGet("{ingredientId}")]
        public IActionResult Get(int ingredientId)
        {
            if (_ingredientService.GetIngredient(ingredientId) is not { } ingredient) return NotFound();
            var result = IngredientDto.Map(ingredient);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll(int recipeId)
        {
            var result = IngredientDto.MapCollection(_ingredientService.GetAllIngredients(recipeId));
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateIngredientModel model)
        {
            if (_ingredientService.CreateIngredient(model) is not { } newIngredient) return NotFound();
            var result = IngredientDto.Map(newIngredient);
            return Ok(result);
        }

        [HttpPost("{ingredientId}")]
        public IActionResult Update(int ingredientId, [FromBody] UpdateIngredientModel model)
        {
            if (_ingredientService.UpdateIngredient(ingredientId, model) is not { } ingredient) return NotFound();
            var result = IngredientDto.Map(ingredient);
            return Ok(result);
        }

        [HttpDelete("{ingredientId}")]
        public IActionResult Delete(int ingredientId)
        {
            if (!_ingredientService.DeleteIngredient(ingredientId)) return NotFound();
            return Ok();
        }
    }
}
