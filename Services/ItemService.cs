using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IItemService
    {
        Item? GetItem(int itemId);

        List<Item> GetAllItems(int itemCollectionId);

        Item CreateItem(CreateItemModel model);

        Item? UpdateItem(int itemId, UpdateItemModel model);

        Item? UpdateFutureItem(int itemId, UpdateItemModel model);

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

        public List<Item> GetAllItems(int itemCollectionId)
        {
            
            return _context.ItemInstances
                .Where(instance => instance.ItemCollectionId == itemCollectionId)
                .Select(instance => instance.Item)
                .Include(item => item.RecurrenceRule)
                .Distinct()
                .ToList();
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

        public Item? UpdateFutureItem(int itemId, UpdateItemModel model)
        {
            if (GetItem(itemId) is not { } item) return null;

            var nameChanged = model.Name is { } name && name != item.Name;
            var frequencyTypeChanged = model.FrequencyType is { } frequencyType && frequencyType != item.RecurrenceRule?.FrequencyType;
            var frequencyChanged = model.Frequency is { } frequency && frequency != item.RecurrenceRule?.Frequency;

            if (!nameChanged && !frequencyTypeChanged && !frequencyChanged) return item;

            var newItem = new Item
            {
                Name = model.Name ?? item.Name,
            };

            _recurrenceRuleService.CreateRecurrenceRule(newItem, model.FrequencyType ?? item.RecurrenceRule?.FrequencyType, model.Frequency ?? item.RecurrenceRule?.Frequency);

            if (item.RecurrenceRule is { } rule) _recurrenceRuleService.DeleteRecurrenceRule(rule);

            _context.Items.Add(newItem);
            _context.SaveChanges();
            return newItem;
        }

        public bool DeleteItem(int itemId)
        {
            if (GetItem(itemId) is not { } item) return false;
            if (item.RecurrenceRule is { } rule) _recurrenceRuleService.DeleteRecurrenceRule(rule);
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
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

    }
}
