using System.ComponentModel.DataAnnotations;
using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Models
{
    public class CreateShoppingItemModel
    {
        public required int ProductId { get; set; }

        [Range(0.0001, 1000000)]
        public required decimal Amount { get; set; }

        public required MeasurementUnit Unit { get; set; }
    }
}
