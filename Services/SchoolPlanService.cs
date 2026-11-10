using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;

namespace HouseholdManager.Services
{
    public interface ISchoolPlanService
    {
        SchoolPlan? GetSchoolPlan(int schoolPlanId);

        List<SchoolPlan> GetAllSchoolPlans(int householdId, DateOnly? weekDate);

        Task<SchoolPlan?> CreateSchoolPlanAsync(int householdId, DateOnly weekDate, string originalFileName, Stream content, CancellationToken cancellationToken);

        Stream? OpenFile(SchoolPlan schoolPlan);

        bool DeleteSchoolPlan(int schoolPlanId);
    }

    public class SchoolPlanService : ISchoolPlanService
    {
        public SchoolPlanService(HouseholdContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        private readonly HouseholdContext _context;
        private readonly IFileStorage _fileStorage;

        public SchoolPlan? GetSchoolPlan(int schoolPlanId)
        {
            return _context.SchoolPlans.FirstOrDefault(plan => plan.Id == schoolPlanId);
        }

        public List<SchoolPlan> GetAllSchoolPlans(int householdId, DateOnly? weekDate)
        {
            var query = _context.SchoolPlans.Where(plan => plan.HouseholdId == householdId);
            if (weekDate is { } date)
            {
                var weekStartDate = GetMondayOfWeek(date);
                query = query.Where(plan => plan.WeekStartDate == weekStartDate);
            }

            return query
                .OrderByDescending(plan => plan.WeekStartDate)
                .ThenByDescending(plan => plan.UploadedDate)
                .ToList();
        }

        public async Task<SchoolPlan?> CreateSchoolPlanAsync(int householdId, DateOnly weekDate, string originalFileName, Stream content, CancellationToken cancellationToken)
        {
            if (!_context.Households.Any(household => household.Id == householdId)) return null;

            var storedFileName = await _fileStorage.SaveAsync(content, AllowedFileKind.ImageOrPdf, cancellationToken);

            var newPlan = new SchoolPlan
            {
                HouseholdId = householdId,
                WeekStartDate = GetMondayOfWeek(weekDate),
                StoredFileName = storedFileName,
                OriginalFileName = GetSafeFileName(originalFileName),
                UploadedDate = DateTimeOffset.UtcNow
            };

            try
            {
                var result = _context.SchoolPlans.Add(newPlan);
                _context.SaveChanges();
                return result.Entity;
            }
            catch
            {
                _fileStorage.Delete(storedFileName);
                throw;
            }
        }

        public Stream? OpenFile(SchoolPlan schoolPlan)
        {
            return _fileStorage.Open(schoolPlan.StoredFileName);
        }

        public bool DeleteSchoolPlan(int schoolPlanId)
        {
            if (GetSchoolPlan(schoolPlanId) is not { } schoolPlan) return false;

            _context.SchoolPlans.Remove(schoolPlan);
            _context.SaveChanges();
            _fileStorage.Delete(schoolPlan.StoredFileName);
            return true;
        }

        private static DateOnly GetMondayOfWeek(DateOnly date)
        {
            var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-daysSinceMonday);
        }

        private static string GetSafeFileName(string fileName)
        {
            var name = Path.GetFileName(fileName);
            if (string.IsNullOrWhiteSpace(name)) return "school-plan";
            return name.Length > 255 ? name[..255] : name;
        }
    }
}
