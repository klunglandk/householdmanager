using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCollectionsController : ControllerBase
    {
        public ItemCollectionsController(IItemCollectionService itemCollectionService) {
            _itemCollectionService = itemCollectionService;
        }

        private readonly IItemCollectionService _itemCollectionService;


        [HttpGet("{itemCollectionId}")]
        public IActionResult Get(int itemCollectionId)
        {
            if(_itemCollectionService.GetItemCollection(itemCollectionId) is not { } itemCollection) return NotFound();
            var result = ItemCollectionDto.Map(itemCollection);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = ItemCollectionDto.MapCollection(_itemCollectionService.GetAllItemCollections());
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CreateItemCollectionModel model)
        {
            var newItemCollection = _itemCollectionService.CreateItemCollection(model);
            var result = ItemCollectionDto.Map(newItemCollection);
            return Ok(result);
        }

        [HttpPost("{itemCollectionId}")]
        public IActionResult Update(int itemCollectionId, [FromBody]UpdateItemCollectionModel model)
        {
            if (_itemCollectionService.UpdateItemCollection(itemCollectionId, model) is not { } itemCollection) return NotFound();
            var result = ItemCollectionDto.Map(itemCollection);
            return Ok(result);
        }

        [HttpDelete("{itemCollectionId}")]
        public IActionResult Delete(int itemCollectionId)
        {
            if (!_itemCollectionService.DeleteItemCollection(itemCollectionId)) return NotFound();
            return Ok();
        }
    }
}
