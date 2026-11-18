using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class UpdateProductModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }
    }
}
