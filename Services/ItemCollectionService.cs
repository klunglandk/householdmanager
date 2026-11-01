using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IItemCollectionService
    {
        ItemCollection CreateItemCollection(CreateItemCollectionModel model);

        ItemCollection? GetItemCollection(int itemCollectionId);

        List<ItemCollection> GetAllItemCollections();

        ItemCollection? UpdateItemCollection(int itemCollectionId, UpdateItemCollectionModel model);

        bool DeleteItemCollection(int itemCollectionId);
    }

    public class ItemCollectionService : IItemCollectionService
    {
        public ItemCollectionService(HouseholdContext context) {
        _context = context;
        }

        private readonly HouseholdContext _context;

        public ItemCollection CreateItemCollection(CreateItemCollectionModel model)
        {
            var result = _context.ItemCollections.Add(new ItemCollection { Name = model.Name, OwnerId = model.OwnerId, Status = ItemCollectionStatus.Normal });
            _context.SaveChanges();
            return result.Entity;
        }

        public ItemCollection? GetItemCollection(int itemCollectionId)
        {
            return _context.ItemCollections.FirstOrDefault(collection => collection.Id == itemCollectionId);
        }

        public List<ItemCollection> GetAllItemCollections()
        {
            return _context.ItemCollections.Where(collection => collection.Status == ItemCollectionStatus.Normal).ToList();
        }

        public ItemCollection? UpdateItemCollection(int itemCollectionId, UpdateItemCollectionModel model)
        {
            if (GetItemCollection(itemCollectionId) is not { } itemCollection) return null;
            if (model.Name is { } name) { itemCollection.Name = name; }
            _context.SaveChanges();
            return itemCollection;
        }

        public bool DeleteItemCollection(int itemCollectionId)
        {
            var result = _context.ItemCollections.Where(collection => collection.Status == ItemCollectionStatus.Normal && collection.Id == itemCollectionId).FirstOrDefault();
            if (result == null)
            {
                return false;
            }

            result.Status = ItemCollectionStatus.Archived;
            _context.SaveChanges();
            return true;
        }
    }

    public class CreateItemCollectionModel
    {
        [Required, MinLength(2)]
        public required string Name { get; set; }

        public int OwnerId { get; set; }
    }

    public class UpdateItemCollectionModel
    {
        [MinLength(2)]
        public string? Name { get; set; }

    }
}
