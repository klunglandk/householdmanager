using HouseholdManager.Dtos;
using HouseholdManager.Infrastructure;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        private readonly IRecipeService _recipeService;

        [HttpGet("{recipeId}")]
        public IActionResult Get(int recipeId)
        {
            if (_recipeService.GetRecipe(recipeId) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll(int householdId)
        {
            var result = RecipeDto.MapCollection(_recipeService.GetAllRecipes(householdId));
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateRecipeModel model)
        {
            var newRecipe = _recipeService.CreateRecipe(model);
            var result = RecipeDto.Map(newRecipe);
            return Ok(result);
        }

        [HttpPost("{recipeId}")]
        public IActionResult Update(int recipeId, [FromBody] UpdateRecipeModel model)
        {
            if (_recipeService.UpdateRecipe(recipeId, model) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }

        [HttpDelete("{recipeId}")]
        public IActionResult Delete(int recipeId)
        {
            if (!_recipeService.DeleteRecipe(recipeId)) return NotFound();
            return Ok();
        }

        [HttpGet("{recipeId}/image")]
        public IActionResult GetImage(int recipeId)
        {
            if (_recipeService.GetRecipe(recipeId) is not { ImagePath: { } imagePath } recipe) return NotFound();
            if (_recipeService.OpenImage(recipe) is not { } stream) return NotFound();

            Response.Headers.XContentTypeOptions = "nosniff";
            return File(stream, FileTypeDetector.GetContentType(imagePath));
        }

        [HttpPost("{recipeId}/image")]
        public async Task<IActionResult> UploadImage(int recipeId, IFormFile file, CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            if (await _recipeService.SetImageAsync(recipeId, stream, cancellationToken) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }

        [HttpDelete("{recipeId}/image")]
        public IActionResult DeleteImage(int recipeId)
        {
            if (_recipeService.RemoveImage(recipeId) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }
    }
}
