using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IItemInstanceService
    {
        ItemInstance? GetItemInstance(int itemInstanceId);

        List<ItemInstance> GetAllItemInstances(int itemCollectionId);

        ItemInstance CreateItemInstance(CreateItemInstanceModel model);

        void MarkAsDone(int itemInstanceId);

        void ClearDoneDate(int itemInstanceId);

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
                .Where(instance => instance.ItemCollectionId == itemCollectionId).ToList();
        }

        public ItemInstance CreateItemInstance(CreateItemInstanceModel model)
        {
            ItemInstance newInstance;

            if (model.ItemId is { } itemId)
            {
                var item = _itemService.GetItem(itemId) ?? throw new InvalidOperationException("Item not found");
                newInstance = new ItemInstance { PeriodStartDate = model.PeriodStartDate, Item = item, ItemCollectionId = model.ItemCollectionId};
            }
            else
            {
                if (model.Name is not { } name) throw new InvalidOperationException("Name is required");
                var itemModel = new CreateItemModel { Name = name, FrequencyType = model.FrequencyType, Frequency = model.Frequency };
                var newItem = _itemService.CreateItem(itemModel);

                newInstance = new ItemInstance { PeriodStartDate = model.PeriodStartDate, Item = newItem, ItemCollectionId = model.ItemCollectionId };
            }

            var result = _context.ItemInstances.Add(newInstance);
            _context.SaveChanges();
            return result.Entity;
        }

        public void MarkAsDone(int itemInstanceId)
        {
            var instance = GetItemInstance(itemInstanceId) ?? throw new InvalidOperationException("Item instance not found");
            instance.DoneDate = DateTimeOffset.UtcNow;
            _context.SaveChanges();
        }

        public void ClearDoneDate(int itemInstanceId)
        {
            var instance = GetItemInstance(itemInstanceId) ?? throw new InvalidOperationException("Item instance not found");
            instance.DoneDate = null;
            _context.SaveChanges();

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
        public int? ItemId { get; set; }

        public required int ItemCollectionId { get; set; }

        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

        public FrequencyType? FrequencyType { get; set; }

        public int Frequency {  get; set; } = 1;

        public required DateTimeOffset PeriodStartDate { get; set; }
    }

}
