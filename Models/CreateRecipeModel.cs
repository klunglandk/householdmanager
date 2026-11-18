using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class CreateRecipeModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public string Instructions { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }
    }
}
