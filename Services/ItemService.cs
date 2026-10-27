using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IItemService
    {
        Item? GetItem(int itemId);

        Item CreateItem(CreateItemModel model);

        Item? UpdateItem(int itemId, UpdateItemModel model);

        bool DeleteRecurrenceRule(int itemId);

        bool DeleteItem(int itemId);
    }
    public class ItemService : IItemService
    {
        public ItemService(HouseholdContext context, IRecurrenceRuleService recurrenceRuleService)
        {
            _context = context;
            _recurrenceRuleService = recurrenceRuleService;
        }
        private readonly HouseholdContext _context;
        private readonly IRecurrenceRuleService _recurrenceRuleService;

        public Item? GetItem(int itemId)
        {
            return _context.Items
                .Include(item => item.RecurrenceRule)
                .FirstOrDefault(item => item.Id == itemId);
        }

        public Item CreateItem(CreateItemModel model)
        {
            var newItem = new Item { Name = model.Name };
            _recurrenceRuleService.CreateRecurrenceRule(newItem, model.FrequencyType, model.Frequency);
            var result = _context.Items.Add(newItem);
            _context.SaveChanges();

            return result.Entity;
        }
        public Item? UpdateItem(int itemId, UpdateItemModel model)
        {
            if (GetItem(itemId) is not { } item) return null;
            if (model.Name is { } name) item.Name = name;
            if (model.FrequencyType is { } frequencyType)
            {
                if (item.RecurrenceRule is { } recurrenceRule)
                {
                    _recurrenceRuleService.UpdateRecurrenceRule(recurrenceRule, frequencyType, model.Frequency);
                }
                else
                {
                    _recurrenceRuleService.CreateRecurrenceRule(item, frequencyType, model.Frequency);
                }
            }
            _context.SaveChanges();
            return item;
        }

        public bool DeleteRecurrenceRule(int itemId)
        {
            if (GetItem(itemId) is not { } item) return false;
            if (item.RecurrenceRule is not { } currentRule) return false;

            _context.RecurrenceRules.Remove(currentRule);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteItem(int itemId)
        {
            if (GetItem(itemId) is not { } item) return false;
            if (item.RecurrenceRule is { } rule) _context.RecurrenceRules.Remove(rule);
            _context.Items.Remove(item);
            _context.SaveChanges();
            return true;
        }
    }

    public class CreateItemModel : RecurrenceInputModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

    }

    public class UpdateItemModel : RecurrenceInputModel
    {
        public string? Name { get; set; }

    }

    public abstract class RecurrenceInputModel : IValidatableObject
    {
        public FrequencyType? FrequencyType { get; set; }

        public int? Frequency { get; set; }
    
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (Frequency is not null && FrequencyType is null)
            {
                yield return new ValidationResult("Frequency requires FrequencyType", [nameof(Frequency)]);
            }
        }
    }
}
