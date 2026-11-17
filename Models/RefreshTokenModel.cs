using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class RefreshTokenModel
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
