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
    public class ProductsController : ControllerBase
    {
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        private readonly IProductService _productService;

        [HttpGet("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductDto> Get(int productId)
        {
            if (_productService.GetProduct(productId) is not { } product) return NotFound();
            var result = ProductDto.Map(product);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductDto>> GetAll()
        {
            var result = ProductDto.MapCollection(_productService.GetAllProducts());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<ProductDto> Create([FromBody] CreateProductModel model)
        {
            var newProduct = _productService.CreateProduct(model);
            var result = ProductDto.Map(newProduct);
            return CreatedAtAction(nameof(Get), new { productId = newProduct.Id }, result);
        }

        [HttpPost("{productId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductDto> Update(int productId, [FromBody] UpdateProductModel model)
        {
            if (_productService.UpdateProduct(productId, model) is not { } product) return NotFound();
            var result = ProductDto.Map(product);
            return Ok(result);
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int productId)
        {
            if (!_productService.DeleteProduct(productId)) return NotFound();
            return NoContent();
        }
    }
}
