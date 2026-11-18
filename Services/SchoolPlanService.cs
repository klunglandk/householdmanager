using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;

namespace HouseholdManager.Services
{
    public class SchoolPlanService : ISchoolPlanService
    {
        public SchoolPlanService(HouseholdContext context, IFileStorage fileStorage, ICurrentUser currentUser)
        {
            _context = context;
            _fileStorage = fileStorage;
            _currentUser = currentUser;
        }

        private readonly HouseholdContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUser _currentUser;

        public SchoolPlan? GetSchoolPlan(int schoolPlanId)
        {
            return _context.SchoolPlans.FirstOrDefault(plan => plan.Id == schoolPlanId && plan.HouseholdId == _currentUser.HouseholdId);
        }

        public List<SchoolPlan> GetAllSchoolPlans(DateOnly? weekDate)
        {
            var query = _context.SchoolPlans.Where(plan => plan.HouseholdId == _currentUser.HouseholdId);
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

        public async Task<SchoolPlan> CreateSchoolPlanAsync(DateOnly weekDate, string originalFileName, Stream content, CancellationToken cancellationToken)
        {
            var storedFileName = await _fileStorage.SaveAsync(content, AllowedFileKind.ImageOrPdf, cancellationToken);

            var newPlan = new SchoolPlan
            {
                HouseholdId = _currentUser.HouseholdId,
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
