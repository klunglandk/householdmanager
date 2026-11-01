using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager.Database
{
    public class HouseholdContext : DbContext
    {

        public HouseholdContext(DbContextOptions<HouseholdContext> options) : base(options)
        {

        }

        public DbSet<ItemCollection> ItemCollections { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<RecurrenceRule> RecurrenceRules { get; set; }

        public DbSet<ItemInstance> ItemInstances { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemCollection>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.Property(p => p.Name).HasMaxLength(128);
                e.HasOne(p => p.Owner).WithMany(p => p.ItemCollections).HasForeignKey(p => p.OwnerId);
                e.HasIndex(p => p.Status);
            }
            );

            modelBuilder.Entity<Item>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.Property(p => p.Name).HasMaxLength(128);
                e.HasOne(p => p.RecurrenceRule).WithOne(p => p.Item).HasForeignKey<Item>(p => p.RecurrenceRuleId);
                e.HasOne(p => p.ItemCollection).WithMany(p => p.Items).HasForeignKey(p => p.ItemCollectionId);
            });

            modelBuilder.Entity<RecurrenceRule>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ItemInstance>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.HasOne(p => p.Item).WithMany(p => p.ItemInstances).HasForeignKey(p => p.ItemId);
            });
        }
    }

}
