using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        public ItemsController(IItemService itemService) {
            _itemService = itemService;
        }

        private readonly IItemService _itemService;

        [HttpGet("{itemId}")]
        public IActionResult Get(int itemId)
        {
            if (_itemService.GetItem(itemId) is not { } item) return NotFound();
            var result = ItemDto.Map(item);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll(int itemCollectionId)
        {
            var result = ItemDto.MapItems(_itemService.GetAllItems(itemCollectionId));
            return Ok(result);
        }

        [HttpPost("{itemId}")]
        public IActionResult Update(int itemId, [FromBody] UpdateItemModel model)
        {
            if (_itemService.UpdateItem(itemId, model) is not { } item) return NotFound();
            var result = ItemDto.Map(item);
            return Ok(result);
        }

    }
}
