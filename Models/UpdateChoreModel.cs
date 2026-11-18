using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class UpdateChoreModel : RecurrenceInputModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

        public string? UserId { get; set; }
    }
}
