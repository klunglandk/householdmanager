using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class CreateUserModel
    {
        [Required]
        public required string UserName { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }
    }
}
