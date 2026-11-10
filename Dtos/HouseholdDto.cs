using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class HouseholdDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public static HouseholdDto Map(Household household)
        {
            var result = new HouseholdDto
            {
                Id = household.Id,
                Name = household.Name
            };
            return result;
        }

        public static IEnumerable<HouseholdDto> MapCollection(IEnumerable<Household> households)
        {
            foreach (var household in households)
            {
                yield return Map(household);
            }
        }
    }
}
