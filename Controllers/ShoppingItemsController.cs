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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public class ShoppingItemsController : ControllerBase
    {
        public ShoppingItemsController(IShoppingItemService shoppingItemService)
        {
            _shoppingItemService = shoppingItemService;
        }

        private readonly IShoppingItemService _shoppingItemService;

        [HttpGet("{shoppingItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShoppingItemDto> Get(int shoppingItemId)
        {
            if (_shoppingItemService.GetShoppingItem(shoppingItemId) is not { } shoppingItem) return NotFound();
            var result = ShoppingItemDto.Map(shoppingItem);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ShoppingItemDto>> GetAll()
        {
            var result = ShoppingItemDto.MapCollection(_shoppingItemService.GetAllShoppingItems());
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShoppingItemDto> Create([FromBody] CreateShoppingItemModel model)
        {
            if (_shoppingItemService.CreateShoppingItem(model) is not { } newShoppingItem) return NotFound();
            var result = ShoppingItemDto.Map(newShoppingItem);
            return Ok(result);
        }

        [HttpPost("from-recipe/{recipeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<ShoppingItemDto>> AddFromRecipe(int recipeId, [FromBody] AddRecipeToShoppingListModel? model = null)
        {
            if (_shoppingItemService.AddFromRecipe(recipeId, model ?? new AddRecipeToShoppingListModel()) is not { } shoppingItems) return NotFound();
            var result = ShoppingItemDto.MapCollection(shoppingItems);
            return Ok(result);
        }

        [HttpPost("{shoppingItemId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShoppingItemDto> Update(int shoppingItemId, [FromBody] UpdateShoppingItemModel model)
        {
            if (_shoppingItemService.UpdateShoppingItem(shoppingItemId, model) is not { } shoppingItem) return NotFound();
            var result = ShoppingItemDto.Map(shoppingItem);
            return Ok(result);
        }

        [HttpPost("{shoppingItemId}/mark-as-bought")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShoppingItemDto> MarkAsBought(int shoppingItemId)
        {
            if (_shoppingItemService.MarkAsBought(shoppingItemId) is not { } shoppingItem) return NotFound();
            return Ok(ShoppingItemDto.Map(shoppingItem));
        }

        [HttpPost("{shoppingItemId}/clear-bought-date")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShoppingItemDto> ClearBoughtDate(int shoppingItemId)
        {
            if (_shoppingItemService.ClearBoughtDate(shoppingItemId) is not { } shoppingItem) return NotFound();
            return Ok(ShoppingItemDto.Map(shoppingItem));
        }

        [HttpDelete("bought")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<DeletedCountDto> DeleteBought()
        {
            var deletedCount = _shoppingItemService.DeleteBoughtShoppingItems();
            return Ok(new DeletedCountDto { DeletedCount = deletedCount });
        }

        [HttpDelete("{shoppingItemId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int shoppingItemId)
        {
            if (!_shoppingItemService.DeleteShoppingItem(shoppingItemId)) return NotFound();
            return NoContent();
        }
    }
}
