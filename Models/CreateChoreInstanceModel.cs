namespace HouseholdManager.Models
{
    public class CreateChoreInstanceModel
    {
        public required int ChoreId { get; set; }

        public required DateTimeOffset PeriodStartDate { get; set; }
    }
}
