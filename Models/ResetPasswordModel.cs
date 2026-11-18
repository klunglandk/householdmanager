using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class ResetPasswordModel
    {
        [Required, MaxLength(128)]
        public required string NewPassword { get; set; }
    }
}
