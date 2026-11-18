using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class CreateHouseholdModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }
    }
}
