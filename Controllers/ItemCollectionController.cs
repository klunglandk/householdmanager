using HouseholdManager.Services;
using Microsoft.AspNetCore.Http;
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
            return Ok(itemCollection);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_itemCollectionService.GetAllItemCollections());
        }

        [HttpPost]
        public IActionResult Create([FromBody]CreateItemCollectionModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var itemCollection = _itemCollectionService.CreateItemCollection(model);
            return Ok(itemCollection);
        }

        [HttpPost("{itemCollectionId}")]
        public IActionResult Update(int itemCollectionId, [FromBody]UpdateItemCollectionModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            if (_itemCollectionService.GetItemCollection(itemCollectionId) is not { } itemCollection) return NotFound();
            _itemCollectionService.UpdateItemCollection(itemCollectionId, model);
            return Ok(itemCollection);
        }

        [HttpDelete("{itemCollectionId}")]
        public IActionResult Delete(int itemCollectionId)
        {
            if (_itemCollectionService.GetItemCollection(itemCollectionId) is not { } itemCollection) return NotFound();
            _itemCollectionService.DeleteItemCollection(itemCollectionId);
            return Ok();
        }
    }
}
