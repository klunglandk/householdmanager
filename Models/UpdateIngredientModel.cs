using System.ComponentModel.DataAnnotations;
using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Models
{
    public class UpdateIngredientModel
    {
        [Range(0.0001, 1000000)]
        public decimal? Amount { get; set; }

        public MeasurementUnit? Unit { get; set; }
    }
}
