using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IItemInstanceService
    {
        ItemInstance? GetItemInstance(int itemInstanceId);

        List<ItemInstance> GetAllItemInstances(int itemCollectionId);

        ItemInstance? CreateItemInstance(CreateItemInstanceModel model);

        ItemInstance CreateItemInstanceWithNewItem(CreateItemInstanceWithNewItemModel model);

        ItemInstance? MarkAsDone(int itemInstanceId);

        ItemInstance? ClearDoneDate(int itemInstanceId);

        bool DeleteItemInstance(int itemInstanceId);

    }
    public class ItemInstanceService : IItemInstanceService
    {
        public ItemInstanceService(HouseholdContext context, IItemService itemService)
        {
            _context = context;
            _itemService = itemService;
        }

        private readonly HouseholdContext _context;
        private readonly IItemService _itemService;

        public ItemInstance? GetItemInstance(int itemInstanceId)
        {
            return _context.ItemInstances
                .Include(instance => instance.Item)
                .ThenInclude(item => item.RecurrenceRule)
                .FirstOrDefault(instance => instance.Id == itemInstanceId);
        }

        public List<ItemInstance> GetAllItemInstances(int itemCollectionId)
        {
            return _context.ItemInstances
                .Include(instance => instance.Item)
                .ThenInclude(item => item.RecurrenceRule)
                .Where(instance => instance.Item.ItemCollectionId == itemCollectionId).ToList();
        }

        public ItemInstance? CreateItemInstance(CreateItemInstanceModel model)
        {

            if (_itemService.GetItem(model.ItemId) is not { } item) return null;

            var newInstance = new ItemInstance
            {
                Item = item,
                PeriodStartDate = model.PeriodStartDate,
            };

            return SaveNewInstance(newInstance);
        }

        public ItemInstance CreateItemInstanceWithNewItem(CreateItemInstanceWithNewItemModel model)
        {
            var itemModel = new CreateItemModel { Name = model.Name, ItemCollectionId = model.ItemCollectionId, FrequencyType = model.FrequencyType, Frequency = model.Frequency, Interval = model.Interval };
            var newItem = _itemService.CreateItem(itemModel);

            var newInstance = new ItemInstance
            {
                PeriodStartDate = model.PeriodStartDate,
                Item = newItem,
            };

            return SaveNewInstance(newInstance);
        }

        private ItemInstance SaveNewInstance(ItemInstance instance)
        {
            var result = _context.ItemInstances.Add(instance);
            _context.SaveChanges();
            return result.Entity;
        }

        public ItemInstance? MarkAsDone(int itemInstanceId)
        {
            if (GetItemInstance(itemInstanceId) is not { } instance) return null;
            instance.DoneDate = DateTimeOffset.UtcNow;
            _context.SaveChanges();
            return instance;
        }

        public ItemInstance? ClearDoneDate(int itemInstanceId)
        {
            if (GetItemInstance(itemInstanceId) is not { } instance) return null;
            instance.DoneDate = null;
            _context.SaveChanges();
            return instance;
        }

        public bool DeleteItemInstance(int itemInstanceId)
        {
            if (GetItemInstance(itemInstanceId) is not { } itemInstance) return false;

            _context.ItemInstances.Remove(itemInstance);
            _context.SaveChanges();

            var count = _context.ItemInstances.Where(instance => instance.ItemId == itemInstance.ItemId).Count();

            if (count == 0)
            {
                _itemService.DeleteItem(itemInstance.ItemId);
            }

            return true;

        }

    }

    public class CreateItemInstanceModel
    {
        public required int ItemId { get; set; }

        public required DateTimeOffset PeriodStartDate { get; set; }
    }

    public class CreateItemInstanceWithNewItemModel : RecurrenceInputModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public required int ItemCollectionId { get; set; }

        public required DateTimeOffset PeriodStartDate { get; set; }
    }

}
