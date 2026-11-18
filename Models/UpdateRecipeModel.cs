using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class UpdateRecipeModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

        public string? Instructions { get; set; }

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }
    }
}
