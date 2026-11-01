using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemInstancesController : ControllerBase
    {
        public ItemInstancesController(IItemInstanceService itemInstanceService)
        {
            _itemInstanceService = itemInstanceService;
        }

        private readonly IItemInstanceService _itemInstanceService;

        [HttpGet("{itemInstanceId}")]
        public IActionResult Get(int itemInstanceId)
        {
            if (_itemInstanceService.GetItemInstance(itemInstanceId) is not { } itemInstance) return NotFound();
            var result = ItemInstanceDto.Map(itemInstance);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll(int itemCollectionId)
        {
            var result = ItemInstanceDto.MapCollection(_itemInstanceService.GetAllItemInstances(itemCollectionId));
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateItemInstanceModel model)
        {
            if (_itemInstanceService.CreateItemInstance(model) is not { } newItemInstance) return NotFound();
            var result = ItemInstanceDto.Map(newItemInstance);
            return Ok(result);
        }

        [HttpPost("new-item")]
        public IActionResult CreateWithNewItem([FromBody] CreateItemInstanceWithNewItemModel model)
        {
            var newItemInstance = _itemInstanceService.CreateItemInstanceWithNewItem(model);
            var result = ItemInstanceDto.Map(newItemInstance);
            return Ok(result);
        }

        [HttpPost("{itemInstanceId}/mark-as-done")]
        public IActionResult MarkAsDone(int itemInstanceId)
        {
            if (_itemInstanceService.MarkAsDone(itemInstanceId) is not { } itemInstance) return NotFound();
            return Ok(itemInstance.Map());
        }

        [HttpPost("{itemInstanceId}/clear-done-date")]
        public IActionResult ClearDoneDate(int itemInstanceId)
        {
            if (_itemInstanceService.ClearDoneDate(itemInstanceId) is not { } itemInstance) return NotFound();
            return Ok(itemInstance.Map());
        }

        [HttpDelete("{itemInstanceId}")]
        public IActionResult Delete(int itemInstanceId)
        {
            if (!_itemInstanceService.DeleteItemInstance(itemInstanceId)) return NotFound();
            return Ok();
        }
    }
}
