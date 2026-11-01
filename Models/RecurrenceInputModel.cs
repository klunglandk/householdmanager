using HouseholdManager.Database.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public abstract class RecurrenceInputModel : IValidatableObject
    {
        public FrequencyType? FrequencyType { get; set; }

        public int? Frequency { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (Frequency is not null && FrequencyType is null)
            {
                yield return new ValidationResult("Frequency requires FrequencyType", [nameof(Frequency)]);
            }
        }
    }
}
