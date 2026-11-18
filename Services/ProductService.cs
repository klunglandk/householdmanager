using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
using HouseholdManager.Models;

namespace HouseholdManager.Services
{
    public class ProductService : IProductService
    {
        public ProductService(HouseholdContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        private readonly HouseholdContext _context;
        private readonly ICurrentUser _currentUser;

        public Product? GetProduct(int productId)
        {
            return _context.Products.Where(product => product.HouseholdId == _currentUser.HouseholdId).FirstOrDefault(product => product.Id == productId);
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Where(product => product.HouseholdId == _currentUser.HouseholdId).ToList();
        }

        public Product CreateProduct(CreateProductModel model)
        {
            var result = _context.Products.Add(new Product { Name = model.Name, HouseholdId = _currentUser.HouseholdId });
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
}
