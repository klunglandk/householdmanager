using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class ChangePasswordModel
    {
        [Required, MaxLength(128)]
        public required string CurrentPassword { get; set; }

        [Required, MaxLength(128)]
        public required string NewPassword { get; set; }
    }
}
