using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;

namespace HouseholdManager.Dtos
{
    public class SchoolPlanDto
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        public DateOnly WeekStartDate { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public DateTimeOffset UploadedDate { get; set; }

        public static SchoolPlanDto Map(SchoolPlan schoolPlan)
        {
            var result = new SchoolPlanDto
            {
                Id = schoolPlan.Id,
                HouseholdId = schoolPlan.HouseholdId,
                WeekStartDate = schoolPlan.WeekStartDate,
                OriginalFileName = schoolPlan.OriginalFileName,
                ContentType = FileTypeDetector.GetContentType(schoolPlan.StoredFileName),
                UploadedDate = schoolPlan.UploadedDate
            };
            return result;
        }

        public static IEnumerable<SchoolPlanDto> MapCollection(IEnumerable<SchoolPlan> schoolPlans)
        {
            foreach (var schoolPlan in schoolPlans)
            {
                yield return Map(schoolPlan);
            }
        }
    }
}
