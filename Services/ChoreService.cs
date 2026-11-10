using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IChoreService
    {
        Chore? GetChore(int choreId);

        List<Chore> GetAllChores(int householdId);

        Chore CreateChore(CreateChoreModel model);

        Chore? UpdateChore(int choreId, UpdateChoreModel model);

        Chore? UpdateFutureChore(int choreId, UpdateChoreModel model);

        bool DeleteChore(int choreId);
    }
    public class ChoreService : IChoreService
    {
        public ChoreService(HouseholdContext context, IRecurrenceRuleService recurrenceRuleService)
        {
            _context = context;
            _recurrenceRuleService = recurrenceRuleService;
        }
        private readonly HouseholdContext _context;
        private readonly IRecurrenceRuleService _recurrenceRuleService;

        public Chore? GetChore(int choreId)
        {
            return _context.Chores
                .Include(chore => chore.RecurrenceRule)
                .FirstOrDefault(chore => chore.Id == choreId);
        }

        public List<Chore> GetAllChores(int householdId)
        {
            return _context.Chores
                .Where(chore => chore.HouseholdId == householdId)
                .Include(chore => chore.RecurrenceRule)
                .ToList();
        }

        public Chore CreateChore(CreateChoreModel model)
        {
            var newChore = new Chore { Name = model.Name, HouseholdId = model.HouseholdId, UserId = model.UserId };
            _recurrenceRuleService.CreateRecurrenceRule(newChore, model.Frequency, model.Interval, model.Weekdays);
            var result = _context.Chores.Add(newChore);
            _context.SaveChanges();

            return result.Entity;
        }

        public Chore? UpdateChore(int choreId, UpdateChoreModel model)
        {
            if (GetChore(choreId) is not { } chore) return null;
            if (model.Name is { } name) chore.Name = name;
            if (model.UserId is { } userId) chore.UserId = userId;
            if (model.Frequency is { } frequency)
            {
                if (chore.RecurrenceRule is { } recurrenceRule)
                {
                    _recurrenceRuleService.UpdateRecurrenceRule(recurrenceRule, frequency, model.Interval, model.Weekdays);
                }
                else
                {
                    _recurrenceRuleService.CreateRecurrenceRule(chore, frequency, model.Interval, model.Weekdays);
                }
            }
            _context.SaveChanges();
            return chore;
        }

        public Chore? UpdateFutureChore(int choreId, UpdateChoreModel model)
        {
            if (GetChore(choreId) is not { } chore) return null;
            if (chore.RecurrenceRule is not { } rule) return null;

            var nameChanged = model.Name is { } name && name != chore.Name;
            var userChanged = model.UserId is { } userId && userId != chore.UserId;
            var frequencyChanged = model.Frequency is { } frequency && frequency != rule.Frequency;
            var intervalChanged = model.Interval is { } interval && interval != rule.Interval;
            var weekdaysChanged = model.Weekdays is { } weekdays && weekdays != rule.Weekdays;

            if (!nameChanged && !userChanged && !frequencyChanged && !intervalChanged && !weekdaysChanged) return chore;

            var newChore = new Chore
            {
                Name = model.Name ?? chore.Name,
                HouseholdId = chore.HouseholdId,
                UserId = model.UserId ?? chore.UserId,
            };

            _recurrenceRuleService.CreateRecurrenceRule(
                newChore,
                model.Frequency ?? rule.Frequency,
                model.Interval ?? rule.Interval,
                model.Weekdays ?? rule.Weekdays
                );

            _recurrenceRuleService.DeleteRecurrenceRule(rule);

            _context.Chores.Add(newChore);
            _context.SaveChanges();
            return newChore;
        }

        public bool DeleteChore(int choreId)
        {
            if (GetChore(choreId) is not { } chore) return false;
            if (chore.RecurrenceRule is { } rule) _recurrenceRuleService.DeleteRecurrenceRule(rule);
            _context.Chores.Remove(chore);
            _context.SaveChanges();
            return true;
        }
    }

    public class CreateChoreModel : RecurrenceInputModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public required int HouseholdId { get; set; }

        public int? UserId { get; set; }

    }

    public class UpdateChoreModel : RecurrenceInputModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

        public int? UserId { get; set; }

    }
}
