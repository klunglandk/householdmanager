namespace HouseholdManager.Models
{
    public class CreateSchoolPlanModel
    {
        public required DateOnly WeekDate { get; set; }

        public required IFormFile File { get; set; }
    }
}
