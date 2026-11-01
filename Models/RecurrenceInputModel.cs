using HouseholdManager.Database.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public abstract class RecurrenceInputModel : IValidatableObject
    {
        public FrequencyType? FrequencyType { get; set; }

        public int? Frequency { get; set; }

        public int? Interval { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if ((Frequency is not null || Interval is not null) && FrequencyType is null)
            {
                yield return new ValidationResult("Frequency and Interval requires FrequencyType", [nameof(Frequency)]);
            }
        }
    }
}
