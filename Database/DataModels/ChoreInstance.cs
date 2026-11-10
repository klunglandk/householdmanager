using HouseholdManager.Dtos;

namespace HouseholdManager.Database.DataModels
{
    public class ChoreInstance
    {
        public int Id { get; set; }

        public DateTimeOffset PeriodStartDate { get; set; }

        public DateTimeOffset? DoneDate { get; set; }

        public bool IsDone => DoneDate != null;

        public DateTimeOffset? SkippedDate { get; set; }

        public bool IsSkipped => SkippedDate != null;

        public Chore Chore { get; set; } = default!;
        public int ChoreId { get; set; }

        public ChoreInstanceDto Map()
        {
            return ChoreInstanceDto.Map(this);
        }
    }
}
