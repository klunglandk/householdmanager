using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HouseholdManager.Services
{
    public class ChoreInstanceGenerator : IChoreInstanceGenerator
    {
        public ChoreInstanceGenerator(HouseholdContext context, IOptions<RecurrenceGenerationOptions> options, TimeProvider timeProvider)
        {
            _context = context;
            _horizonDays = options.Value.HorizonDays;
            _timeZone = options.Value.GetTimeZone();
            _timeProvider = timeProvider;
        }

        private readonly HouseholdContext _context;
        private readonly int _horizonDays;
        private readonly TimeZoneInfo _timeZone;
        private readonly TimeProvider _timeProvider;

        public DateOnly GetToday()
        {
            return GetLocalDate(_timeProvider.GetUtcNow());
        }

        public int GenerateInstances(DateOnly rangeStart, DateOnly rangeEndExclusive)
        {
            var chores = _context.Chores
                .Include(chore => chore.RecurrenceRule)
                .Where(chore => chore.RecurrenceRule != null)
                .ToList();

            var createdCount = StageInstances(chores, rangeStart, rangeEndExclusive, null);
            if (createdCount > 0) _context.SaveChanges();
            return createdCount;
        }

        public int StageInstancesForChore(Chore chore, DateOnly fromDate, Chore? replacedChore = null)
        {
            var rangeEndExclusive = GetToday().AddDays(_horizonDays);
            return StageInstances([chore], fromDate, rangeEndExclusive, replacedChore);
        }

        public int StageRemovalOfOpenInstances(Chore chore, DateOnly fromDate)
        {
            var fromBound = GetStartOfDay(fromDate);
            var instances = _context.ChoreInstances
                .Where(instance => instance.ChoreId == chore.Id
                    && instance.DoneDate == null
                    && instance.SkippedDate == null
                    && instance.PeriodStartDate >= fromBound)
                .ToList();

            _context.ChoreInstances.RemoveRange(instances);
            return instances.Count;
        }

        private int StageInstances(List<Chore> chores, DateOnly rangeStart, DateOnly rangeEndExclusive, Chore? replacedChore)
        {
            if (rangeStart >= rangeEndExclusive) return 0;

            var choresWithRule = chores.Where(chore => chore.RecurrenceRule is not null).ToList();
            if (choresWithRule.Count == 0) return 0;

            var choreIds = choresWithRule.Select(chore => chore.Id).ToList();
            var replacedChoreId = replacedChore?.Id;
            var queriedChoreIds = replacedChoreId is { } id ? choreIds.Append(id).ToList() : choreIds;
            var rangeStartBound = GetStartOfDay(rangeStart);
            var rangeEndBound = GetStartOfDay(rangeEndExclusive);

            var removedInstanceIds = _context.ChangeTracker.Entries<ChoreInstance>()
                .Where(entry => entry.State == EntityState.Deleted)
                .Select(entry => entry.Entity.Id)
                .ToHashSet();

            var existingInstances = _context.ChoreInstances
                .Where(instance => queriedChoreIds.Contains(instance.ChoreId)
                    && instance.PeriodStartDate >= rangeStartBound
                    && instance.PeriodStartDate < rangeEndBound)
                .Select(instance => new { instance.Id, instance.ChoreId, instance.PeriodStartDate })
                .ToList();

            var knownInstances = existingInstances
                .Where(instance => !removedInstanceIds.Contains(instance.Id))
                .Select(instance => (ChoreId: instance.ChoreId == replacedChoreId ? choresWithRule[0].Id : instance.ChoreId, Date: GetLocalDate(instance.PeriodStartDate)))
                .ToHashSet();

            var createdCount = 0;
            foreach (var chore in choresWithRule)
            {
                var occurrences = RecurrenceCalculator.GetOccurrences(chore.RecurrenceRule!, rangeStart, rangeEndExclusive);
                foreach (var date in occurrences)
                {
                    if (!knownInstances.Add((chore.Id, date))) continue;

                    _context.ChoreInstances.Add(new ChoreInstance { Chore = chore, PeriodStartDate = GetStartOfDay(date) });
                    createdCount++;
                }
            }

            return createdCount;
        }

        private DateTimeOffset GetStartOfDay(DateOnly date)
        {
            var localMidnight = date.ToDateTime(TimeOnly.MinValue);
            return new DateTimeOffset(localMidnight, _timeZone.GetUtcOffset(localMidnight));
        }

        private DateOnly GetLocalDate(DateTimeOffset value)
        {
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(value, _timeZone).DateTime);
        }
    }
}
