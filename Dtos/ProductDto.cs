using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int HouseholdId { get; set; }

        public static ProductDto Map(Product product)
        {
            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                HouseholdId = product.HouseholdId
            };
            return result;
        }

        public static IEnumerable<ProductDto> MapCollection(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                yield return Map(product);
            }
        }
    }
}
