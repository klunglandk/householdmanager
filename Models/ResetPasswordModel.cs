using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public required string NewPassword { get; set; }
    }
}
