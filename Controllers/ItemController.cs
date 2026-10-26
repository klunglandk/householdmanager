using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        public ItemController(IItemService itemService) {
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

        [HttpPost]
        public IActionResult Create([FromBody]CreateItemModel model)
        {
            var newItem = _itemService.CreateItem(model);
            var result = ItemDto.Map(newItem);
            return Ok(result);
        }

        [HttpPost("{itemId}")]
        public IActionResult Update(int itemId, [FromBody] UpdateItemModel model)
        {
            if (_itemService.UpdateItem(itemId, model) is not { } item) return NotFound();
            var result = ItemDto.Map(item);
            return Ok(result);
        }

        [HttpDelete("{itemId}")]
        public IActionResult DeleteItem(int itemId)
        {
            if (!_itemService.DeleteItem(itemId)) return NotFound();
            return Ok();
        }

        [HttpDelete("{itemId}")]
        public IActionResult DeleteRule(int itemId)
        {
            if (!_itemService.DeleteRecurrenceRule(itemId)) return NotFound();
            return Ok();
        }

    }
}
