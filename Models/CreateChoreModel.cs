using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class CreateChoreModel : RecurrenceInputModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public string? UserId { get; set; }
    }
}
