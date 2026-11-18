using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Services
{
    public interface ISchoolPlanService
    {
        SchoolPlan? GetSchoolPlan(int schoolPlanId);

        List<SchoolPlan> GetAllSchoolPlans(DateOnly? weekDate);

        Task<SchoolPlan> CreateSchoolPlanAsync(DateOnly weekDate, string originalFileName, Stream content, CancellationToken cancellationToken);

        Stream? OpenFile(SchoolPlan schoolPlan);

        bool DeleteSchoolPlan(int schoolPlanId);
    }
}
