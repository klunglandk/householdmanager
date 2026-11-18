using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class LoginModel
    {
        [Required]
        public required string UserName { get; set; }

        [Required, MaxLength(128)]
        public required string Password { get; set; }
    }
}
