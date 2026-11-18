using System.ComponentModel.DataAnnotations;
using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Models
{
    public abstract class RecurrenceInputModel : IValidatableObject
    {
        public FrequencyType? Frequency { get; set; }

        [Range(1, int.MaxValue)]
        public int? Interval { get; set; }

        public Weekdays? Weekdays { get; set; }

        public DateOnly? StartDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if ((Interval is not null || Weekdays is not null || StartDate is not null) && Frequency is null)
            {
                yield return new ValidationResult("Interval, Weekdays and StartDate requires Frequency", [nameof(Frequency)]);
            }
        }
    }
}
