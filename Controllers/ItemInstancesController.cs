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
            return Ok(itemInstance);
        }

        [HttpGet]
        public IActionResult GetAll(int itemCollectionId)
        {
            return Ok(_itemInstanceService.GetAllItemInstances(itemCollectionId));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateItemInstanceModel model)
        {
            var newItemInstance = _itemInstanceService.CreateItemInstance(model);
            return Ok(newItemInstance);
        }

        [HttpPost("{itemInstanceId}/mark-as-done")]
        public IActionResult MarkAsDone(int itemInstanceId)
        {
            if (_itemInstanceService.GetItemInstance(itemInstanceId) is not { } itemInstance) return NotFound();
            _itemInstanceService.MarkAsDone(itemInstanceId);
            return Ok(itemInstance);
        }

        [HttpPost("{itemInstanceId}/clear-done-date")]
        public IActionResult ClearDoneDate(int itemInstanceId)
        {
            if (_itemInstanceService.GetItemInstance(itemInstanceId) is not { } itemInstance) return NotFound();
            _itemInstanceService.ClearDoneDate(itemInstanceId);
            return Ok(itemInstance);
        }

        [HttpDelete("{itemInstanceId}")]
        public IActionResult Delete(int itemInstanceId)
        {
            if (!_itemInstanceService.DeleteItemInstance(itemInstanceId)) return NotFound();
            return Ok();
        }
    }
}
