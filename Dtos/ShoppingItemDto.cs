using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class ShoppingItemDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public MeasurementUnit Unit { get; set; }

        public DateTimeOffset? BoughtDate { get; set; }

        public bool IsBought => BoughtDate != null;

        public static ShoppingItemDto Map(ShoppingItem shoppingItem)
        {
            var result = new ShoppingItemDto
            {
                Id = shoppingItem.Id,
                ProductId = shoppingItem.ProductId,
                ProductName = shoppingItem.Product.Name,
                Amount = shoppingItem.Amount,
                Unit = shoppingItem.Unit,
                BoughtDate = shoppingItem.BoughtDate
            };
            return result;
        }

        public static IEnumerable<ShoppingItemDto> MapCollection(IEnumerable<ShoppingItem> shoppingItems)
        {
            foreach (var shoppingItem in shoppingItems)
            {
                yield return Map(shoppingItem);
            }
        }
    }
}
