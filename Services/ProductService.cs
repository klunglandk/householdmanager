using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IProductService
    {
        Product? GetProduct(int productId);

        List<Product> GetAllProducts(int householdId);

        Product CreateProduct(CreateProductModel model);

        Product? UpdateProduct(int productId, UpdateProductModel model);

        bool DeleteProduct(int productId);
    }

    public class ProductService : IProductService
    {
        public ProductService(HouseholdContext context)
        {
            _context = context;
        }

        private readonly HouseholdContext _context;

        public Product? GetProduct(int productId)
        {
            return _context.Products.FirstOrDefault(product => product.Id == productId);
        }

        public List<Product> GetAllProducts(int householdId)
        {
            return _context.Products.Where(product => product.HouseholdId == householdId).ToList();
        }

        public Product CreateProduct(CreateProductModel model)
        {
            var result = _context.Products.Add(new Product { Name = model.Name, HouseholdId = model.HouseholdId });
            _context.SaveChanges();
            return result.Entity;
        }

        public Product? UpdateProduct(int productId, UpdateProductModel model)
        {
            if (GetProduct(productId) is not { } product) return null;
            if (model.Name is { } name) product.Name = name;
            _context.SaveChanges();
            return product;
        }

        public bool DeleteProduct(int productId)
        {
            if (GetProduct(productId) is not { } product) return false;
            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }
    }

    public class CreateProductModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public required int HouseholdId { get; set; }
    }

    public class UpdateProductModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }
    }
}
