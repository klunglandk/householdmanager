using HouseholdManager.Dtos;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseholdManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        private readonly IProductService _productService;

        [HttpGet("{productId}")]
        public IActionResult Get(int productId)
        {
            if (_productService.GetProduct(productId) is not { } product) return NotFound();
            var result = ProductDto.Map(product);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = ProductDto.MapCollection(_productService.GetAllProducts());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Create([FromBody] CreateProductModel model)
        {
            var newProduct = _productService.CreateProduct(model);
            var result = ProductDto.Map(newProduct);
            return Ok(result);
        }

        [HttpPost("{productId}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int productId, [FromBody] UpdateProductModel model)
        {
            if (_productService.UpdateProduct(productId, model) is not { } product) return NotFound();
            var result = ProductDto.Map(product);
            return Ok(result);
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int productId)
        {
            if (!_productService.DeleteProduct(productId)) return NotFound();
            return Ok();
        }
    }
}
