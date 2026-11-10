using HouseholdManager.Database.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public abstract class RecurrenceInputModel : IValidatableObject
    {
        public FrequencyType? Frequency { get; set; }

        public int? Interval { get; set; }

        public Weekdays? Weekdays { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if ((Interval is not null || Weekdays is not null) && Frequency is null)
            {
                yield return new ValidationResult("Interval and Weekdays requires Frequency", [nameof(Frequency)]);
            }
        }
    }
}
