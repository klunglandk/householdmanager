using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager.Database
{
    public class HouseholdContext : DbContext
    {

        public HouseholdContext(DbContextOptions<HouseholdContext> options) : base(options)
        {

        }

        public DbSet<Household> Households { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Chore> Chores { get; set; }

        public DbSet<ChoreInstance> ChoreInstances { get; set; }

        public DbSet<RecurrenceRule> RecurrenceRules { get; set; }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<ShoppingItem> ShoppingItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Household>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.Property(p => p.Name).HasMaxLength(128);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.HasOne(p => p.Household).WithMany(p => p.Users).HasForeignKey(p => p.HouseholdId);
            });

            modelBuilder.Entity<Chore>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.Property(p => p.Name).HasMaxLength(128);
                e.HasOne(p => p.RecurrenceRule).WithOne(p => p.Chore).HasForeignKey<Chore>(p => p.RecurrenceRuleId);
                e.HasOne(p => p.Household).WithMany(p => p.Chores).HasForeignKey(p => p.HouseholdId);
                e.HasOne(p => p.User).WithMany(p => p.Chores).HasForeignKey(p => p.UserId);
            });

            modelBuilder.Entity<RecurrenceRule>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ChoreInstance>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.HasOne(p => p.Chore).WithMany(p => p.ChoreInstances).HasForeignKey(p => p.ChoreId);
            });

            modelBuilder.Entity<Recipe>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.Property(p => p.Name).HasMaxLength(128);
                e.HasOne(p => p.Household).WithMany(p => p.Recipes).HasForeignKey(p => p.HouseholdId);
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.Property(p => p.Name).HasMaxLength(128);
                e.HasOne(p => p.Household).WithMany(p => p.Products).HasForeignKey(p => p.HouseholdId);
            });

            modelBuilder.Entity<Ingredient>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.HasOne(p => p.Recipe).WithMany(p => p.Ingredients).HasForeignKey(p => p.RecipeId);
                e.HasOne(p => p.Product).WithMany(p => p.Ingredients).HasForeignKey(p => p.ProductId);
            });

            modelBuilder.Entity<ShoppingItem>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedOnAdd();
                e.HasOne(p => p.Product).WithMany(p => p.ShoppingItems).HasForeignKey(p => p.ProductId);
            });
        }
    }

}
