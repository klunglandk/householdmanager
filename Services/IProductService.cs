using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;

namespace HouseholdManager.Services
{
    public interface IProductService
    {
        Product? GetProduct(int productId);

        List<Product> GetAllProducts();

        Product CreateProduct(CreateProductModel model);

        Product? UpdateProduct(int productId, UpdateProductModel model);

        bool DeleteProduct(int productId);
    }
}
