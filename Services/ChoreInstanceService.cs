using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IChoreInstanceService
    {
        ChoreInstance? GetChoreInstance(int choreInstanceId);

        List<ChoreInstance> GetAllChoreInstances(int householdId);

        ChoreInstance? CreateChoreInstance(CreateChoreInstanceModel model);

        ChoreInstance CreateChoreInstanceWithNewChore(CreateChoreInstanceWithNewChoreModel model);

        ChoreInstance? MarkAsDone(int choreInstanceId);

        ChoreInstance? ClearDoneDate(int choreInstanceId);

        bool DeleteChoreInstance(int choreInstanceId);

    }
    public class ChoreInstanceService : IChoreInstanceService
    {
        public ChoreInstanceService(HouseholdContext context, IChoreService choreService)
        {
            _context = context;
            _choreService = choreService;
        }

        private readonly HouseholdContext _context;
        private readonly IChoreService _choreService;

        public ChoreInstance? GetChoreInstance(int choreInstanceId)
        {
            return _context.ChoreInstances
                .Include(instance => instance.Chore)
                .ThenInclude(chore => chore.RecurrenceRule)
                .FirstOrDefault(instance => instance.Id == choreInstanceId);
        }

        public List<ChoreInstance> GetAllChoreInstances(int householdId)
        {
            return _context.ChoreInstances
                .Include(instance => instance.Chore)
                .ThenInclude(chore => chore.RecurrenceRule)
                .Where(instance => instance.Chore.HouseholdId == householdId).ToList();
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
                HouseholdId = model.HouseholdId,
                UserId = model.UserId,
                Frequency = model.Frequency,
                Interval = model.Interval,
                Weekdays = model.Weekdays
            };
            var newChore = _choreService.CreateChore(choreModel);

            var newInstance = new ChoreInstance
            {
                PeriodStartDate = model.PeriodStartDate,
                Chore = newChore,
            };

            return SaveNewInstance(newInstance);
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

        public bool DeleteChoreInstance(int choreInstanceId)
        {
            if (GetChoreInstance(choreInstanceId) is not { } choreInstance) return false;

            _context.ChoreInstances.Remove(choreInstance);
            _context.SaveChanges();

            var count = _context.ChoreInstances.Where(instance => instance.ChoreId == choreInstance.ChoreId).Count();

            if (count == 0)
            {
                _choreService.DeleteChore(choreInstance.ChoreId);
            }

            return true;

        }

    }

    public class CreateChoreInstanceModel
    {
        public required int ChoreId { get; set; }

        public required DateTimeOffset PeriodStartDate { get; set; }
    }

    public class CreateChoreInstanceWithNewChoreModel : RecurrenceInputModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public required int HouseholdId { get; set; }

        public int? UserId { get; set; }

        public required DateTimeOffset PeriodStartDate { get; set; }
    }

}
