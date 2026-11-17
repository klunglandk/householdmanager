using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
using HouseholdManager.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IChoreService
    {
        Chore? GetChore(int choreId);

        List<Chore> GetAllChores(string? userId);

        Chore CreateChore(CreateChoreModel model);

        Chore? UpdateChore(int choreId, UpdateChoreModel model);

        Chore? UpdateFutureChore(int choreId, UpdateChoreModel model);

        Chore? EndChore(int choreId);

        bool DeleteChore(int choreId);
    }
    public class ChoreService : IChoreService
    {
        public ChoreService(HouseholdContext context, IRecurrenceRuleService recurrenceRuleService, IChoreInstanceGenerator instanceGenerator, ICurrentUser currentUser)
        {
            _context = context;
            _recurrenceRuleService = recurrenceRuleService;
            _instanceGenerator = instanceGenerator;
            _currentUser = currentUser;
        }
        private readonly HouseholdContext _context;
        private readonly IRecurrenceRuleService _recurrenceRuleService;
        private readonly IChoreInstanceGenerator _instanceGenerator;
        private readonly ICurrentUser _currentUser;

        public Chore? GetChore(int choreId)
        {
            return _context.Chores
                .Include(chore => chore.RecurrenceRule)
                .Where(chore => chore.HouseholdId == _currentUser.HouseholdId)
                .Where(chore => _currentUser.IsAdmin || chore.UserId == _currentUser.UserId)
                .FirstOrDefault(chore => chore.Id == choreId);
        }

        public List<Chore> GetAllChores(string? userId)
        {
            var query = _context.Chores.Where(chore => chore.HouseholdId == _currentUser.HouseholdId);
            if (!_currentUser.IsAdmin) userId = _currentUser.UserId;
            if (userId is { } id) query = query.Where(chore => chore.UserId == id);

            return query
                .Include(chore => chore.RecurrenceRule)
                .ToList();
        }

        public Chore CreateChore(CreateChoreModel model)
        {
            EnsureUserInHousehold(model.UserId);
            var newChore = new Chore { Name = model.Name, HouseholdId = _currentUser.HouseholdId, UserId = model.UserId };
            _recurrenceRuleService.CreateRecurrenceRule(newChore, model.Frequency, model.Interval, model.Weekdays, model.StartDate);
            var result = _context.Chores.Add(newChore);
            _context.SaveChanges();

            return result.Entity;
        }

        public Chore? UpdateChore(int choreId, UpdateChoreModel model)
        {
            if (GetChore(choreId) is not { } chore) return null;
            EnsureUserInHousehold(model.UserId);
            if (model.Name is { } name) chore.Name = name;
            if (model.UserId is { } userId) chore.UserId = userId;
            if (model.Frequency is { } frequency)
            {
                var today = _instanceGenerator.GetToday();
                if (chore.RecurrenceRule is { } recurrenceRule)
                {
                    _recurrenceRuleService.UpdateRecurrenceRule(recurrenceRule, frequency, model.Interval, model.Weekdays, model.StartDate);
                    _instanceGenerator.StageRemovalOfOpenInstances(chore, today);
                }
                else
                {
                    _recurrenceRuleService.CreateRecurrenceRule(chore, frequency, model.Interval, model.Weekdays, model.StartDate);
                }
                _instanceGenerator.StageInstancesForChore(chore, today);
            }
            _context.SaveChanges();
            return chore;
        }

        public Chore? UpdateFutureChore(int choreId, UpdateChoreModel model)
        {
            if (GetChore(choreId) is not { } chore) return null;
            if (chore.RecurrenceRule is not { } rule) return null;
            EnsureUserInHousehold(model.UserId);

            var nameChanged = model.Name is { } name && name != chore.Name;
            var userChanged = model.UserId is { } userId && userId != chore.UserId;
            var frequencyChanged = model.Frequency is { } frequency && frequency != rule.Frequency;
            var intervalChanged = model.Interval is { } interval && interval != rule.Interval;
            var weekdaysChanged = model.Weekdays is { } weekdays && weekdays != rule.Weekdays;
            var startDateChanged = model.StartDate is { } startDate && startDate != rule.StartDate;

            if (!nameChanged && !userChanged && !frequencyChanged && !intervalChanged && !weekdaysChanged && !startDateChanged) return chore;

            var newChore = new Chore
            {
                Name = model.Name ?? chore.Name,
                HouseholdId = chore.HouseholdId,
                UserId = model.UserId ?? chore.UserId,
            };

            var newFrequency = model.Frequency ?? rule.Frequency;
            var newWeekdays = model.Weekdays ?? (newFrequency == FrequencyType.Weekly ? rule.Weekdays : null);

            _recurrenceRuleService.CreateRecurrenceRule(
                newChore,
                newFrequency,
                model.Interval ?? rule.Interval,
                newWeekdays,
                model.StartDate ?? rule.StartDate
                );

            _recurrenceRuleService.DeleteRecurrenceRule(rule);

            var firstDateForNewChore = _instanceGenerator.GetToday().AddDays(1);
            _instanceGenerator.StageRemovalOfOpenInstances(chore, firstDateForNewChore);
            _context.Chores.Add(newChore);
            _instanceGenerator.StageInstancesForChore(newChore, firstDateForNewChore, chore);
            _context.SaveChanges();
            return newChore;
        }

        public Chore? EndChore(int choreId)
        {
            if (GetChore(choreId) is not { } chore) return null;
            if (chore.RecurrenceRule is not { } rule) return chore;

            var firstDateToRemove = _instanceGenerator.GetToday().AddDays(1);
            _recurrenceRuleService.DeleteRecurrenceRule(rule);
            _instanceGenerator.StageRemovalOfOpenInstances(chore, firstDateToRemove);
            _context.SaveChanges();
            return chore;
        }

        public bool DeleteChore(int choreId)
        {
            if (GetChore(choreId) is not { } chore) return false;
            if (chore.RecurrenceRule is { } rule) _recurrenceRuleService.DeleteRecurrenceRule(rule);
            _context.Chores.Remove(chore);
            _context.SaveChanges();
            return true;
        }

        private void EnsureUserInHousehold(string? userId)
        {
            if (userId is null) return;
            if (!_context.Users.Any(user => user.Id == userId && user.HouseholdId == _currentUser.HouseholdId))
            {
                throw new ValidationException("The user does not belong to your household.");
            }
        }
    }

    public class CreateChoreModel : RecurrenceInputModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public string? UserId { get; set; }
    }

    public class UpdateChoreModel : RecurrenceInputModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

        public string? UserId { get; set; }

    }
}
