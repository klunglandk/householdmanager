namespace HouseholdManager.Database.DataModels
{
    public class ShoppingItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = default!;

        public decimal Amount { get; set; }

        public MeasurementUnit Unit { get; set; }

        public DateTimeOffset? BoughtDate { get; set; }

        public bool IsBought => BoughtDate != null;

    }
}
