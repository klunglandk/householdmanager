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
    public class RecipesController : ControllerBase
    {
        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        private readonly IRecipeService _recipeService;

        [HttpGet("{recipeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RecipeDto> Get(int recipeId)
        {
            if (_recipeService.GetRecipe(recipeId) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<RecipeDto>> GetAll()
        {
            var result = RecipeDto.MapCollection(_recipeService.GetAllRecipes());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<RecipeDto> Create([FromBody] CreateRecipeModel model)
        {
            var newRecipe = _recipeService.CreateRecipe(model);
            var result = RecipeDto.Map(newRecipe);
            return CreatedAtAction(nameof(Get), new { recipeId = newRecipe.Id }, result);
        }

        [HttpPost("{recipeId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RecipeDto> Update(int recipeId, [FromBody] UpdateRecipeModel model)
        {
            if (_recipeService.UpdateRecipe(recipeId, model) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }

        [HttpDelete("{recipeId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int recipeId)
        {
            if (!_recipeService.DeleteRecipe(recipeId)) return NotFound();
            return NoContent();
        }

        [HttpGet("{recipeId}/image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetImage(int recipeId)
        {
            if (_recipeService.GetRecipe(recipeId) is not { ImagePath: { } imagePath } recipe) return NotFound();
            if (_recipeService.OpenImage(recipe) is not { } stream) return NotFound();

            Response.Headers.XContentTypeOptions = "nosniff";
            return File(stream, FileTypeDetector.GetContentType(imagePath));
        }

        [HttpPost("{recipeId}/image")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecipeDto>> UploadImage(int recipeId, IFormFile file, CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            if (await _recipeService.SetImageAsync(recipeId, stream, cancellationToken) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }

        [HttpDelete("{recipeId}/image")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RecipeDto> DeleteImage(int recipeId)
        {
            if (_recipeService.RemoveImage(recipeId) is not { } recipe) return NotFound();
            var result = RecipeDto.Map(recipe);
            return Ok(result);
        }
    }
}
