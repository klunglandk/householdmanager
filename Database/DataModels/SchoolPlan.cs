namespace HouseholdManager.Database.DataModels
{
    public class SchoolPlan
    {
        public int Id { get; set; }

        public DateOnly WeekStartDate { get; set; }

        public string StoredFileName { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public DateTimeOffset UploadedDate { get; set; }

        public int HouseholdId { get; set; }

        public Household Household { get; set; } = default!;

    }
}
