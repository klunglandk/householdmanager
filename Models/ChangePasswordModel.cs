using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        public required string NewPassword { get; set; }
    }
}
