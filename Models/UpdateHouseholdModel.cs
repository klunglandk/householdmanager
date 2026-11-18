using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class UpdateHouseholdModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }
    }
}
