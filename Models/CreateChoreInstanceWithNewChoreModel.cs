using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class CreateChoreInstanceWithNewChoreModel : RecurrenceInputModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public string? UserId { get; set; }

        public required DateTimeOffset PeriodStartDate { get; set; }
    }
}
