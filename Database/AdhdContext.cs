using KermAdhd.Database.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace KermAdhd.Database;

public class DateTimeUtcNowValueGenerator : ValueGenerator<DateTimeOffset>
{
    public override bool GeneratesTemporaryValues => false;

    public override DateTimeOffset Next(EntityEntry entry)
    {
        return DateTimeOffset.UtcNow;
    }
}

public class AdhdContext : DbContext
{
    public AdhdContext(DbContextOptions<AdhdContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Reminder>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).ValueGeneratedOnAdd();

            e.Property(p => p.Name).HasMaxLength(256);
            e.Property(p => p.CreatedAt).HasValueGenerator<DateTimeUtcNowValueGenerator>().ValueGeneratedOnAdd();

            e.HasIndex(p => p.IsCompleted);
        });

    }

    public DbSet<Reminder> Reminders { get; set; }
}
