using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
using HouseholdManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager.Services
{
    public class ChoreInstanceService : IChoreInstanceService
    {
        public ChoreInstanceService(HouseholdContext context, IChoreService choreService, IChoreInstanceGenerator instanceGenerator, ICurrentUser currentUser)
        {
            _context = context;
            _choreService = choreService;
            _instanceGenerator = instanceGenerator;
            _currentUser = currentUser;
        }

        private readonly HouseholdContext _context;
        private readonly IChoreService _choreService;
        private readonly IChoreInstanceGenerator _instanceGenerator;
        private readonly ICurrentUser _currentUser;

        public ChoreInstance? GetChoreInstance(int choreInstanceId)
        {
            return _context.ChoreInstances
                .Include(instance => instance.Chore)
                .ThenInclude(chore => chore.RecurrenceRule)
                .Where(instance => instance.Chore.HouseholdId == _currentUser.HouseholdId)
                .Where(instance => _currentUser.IsAdmin || instance.Chore.UserId == _currentUser.UserId)
                .FirstOrDefault(instance => instance.Id == choreInstanceId);
        }

        public List<ChoreInstance> GetAllChoreInstances(DateTimeOffset? from, DateTimeOffset? to, string? userId, bool onlyOpen)
        {
            if (!_currentUser.IsAdmin) userId = _currentUser.UserId;

            var query = _context.ChoreInstances
                .Include(instance => instance.Chore)
                .ThenInclude(chore => chore.RecurrenceRule)
                .Where(instance => instance.Chore.HouseholdId == _currentUser.HouseholdId);

            if (from is { } fromDate) query = query.Where(instance => instance.PeriodStartDate >= fromDate);
            if (to is { } toDate) query = query.Where(instance => instance.PeriodStartDate < toDate);
            if (userId is { } id) query = query.Where(instance => instance.Chore.UserId == id);
            if (onlyOpen) query = query.Where(instance => instance.DoneDate == null && instance.SkippedDate == null);

            return query.ToList();
        }

        public ChoreInstance? CreateChoreInstance(CreateChoreInstanceModel model)
        {
            if (_choreService.GetChore(model.ChoreId) is not { } chore) return null;

            var newInstance = new ChoreInstance
            {
                Chore = chore,
                PeriodStartDate = model.PeriodStartDate,
            };

            return SaveNewInstance(newInstance);
        }

        public ChoreInstance CreateChoreInstanceWithNewChore(CreateChoreInstanceWithNewChoreModel model)
        {
            var choreModel = new CreateChoreModel
            {
                Name = model.Name,
                UserId = model.UserId,
                Frequency = model.Frequency,
                Interval = model.Interval,
                Weekdays = model.Weekdays,
                StartDate = model.StartDate ?? DateOnly.FromDateTime(model.PeriodStartDate.DateTime)
            };
            var newChore = _choreService.CreateChore(choreModel);

            var newInstance = new ChoreInstance
            {
                PeriodStartDate = model.PeriodStartDate,
                Chore = newChore,
            };

            var savedInstance = SaveNewInstance(newInstance);

            if (_instanceGenerator.StageInstancesForChore(newChore, _instanceGenerator.GetToday()) > 0)
            {
                _context.SaveChanges();
            }

            return savedInstance;
        }

        private ChoreInstance SaveNewInstance(ChoreInstance instance)
        {
            var result = _context.ChoreInstances.Add(instance);
            _context.SaveChanges();
            return result.Entity;
        }

        public ChoreInstance? MarkAsDone(int choreInstanceId)
        {
            if (GetChoreInstance(choreInstanceId) is not { } instance) return null;
            instance.DoneDate = DateTimeOffset.UtcNow;
            instance.SkippedDate = null;
            _context.SaveChanges();
            return instance;
        }

        public ChoreInstance? ClearDoneDate(int choreInstanceId)
        {
            if (GetChoreInstance(choreInstanceId) is not { } instance) return null;
            instance.DoneDate = null;
            _context.SaveChanges();
            return instance;
        }

        public ChoreInstance? MarkAsSkipped(int choreInstanceId)
        {
            if (GetChoreInstance(choreInstanceId) is not { } instance) return null;
            instance.SkippedDate = DateTimeOffset.UtcNow;
            instance.DoneDate = null;
            _context.SaveChanges();
            return instance;
        }

        public ChoreInstance? ClearSkippedDate(int choreInstanceId)
        {
            if (GetChoreInstance(choreInstanceId) is not { } instance) return null;
            instance.SkippedDate = null;
            _context.SaveChanges();
            return instance;
        }

        public bool DeleteChoreInstance(int choreInstanceId)
        {
            if (GetChoreInstance(choreInstanceId) is not { } choreInstance) return false;

            _context.ChoreInstances.Remove(choreInstance);
            _context.SaveChanges();

            var count = _context.ChoreInstances.Where(instance => instance.ChoreId == choreInstance.ChoreId).Count();

            if (count == 0 && choreInstance.Chore.RecurrenceRule is null)
            {
                _choreService.DeleteChore(choreInstance.ChoreId);
            }

            return true;
        }
    }
}
